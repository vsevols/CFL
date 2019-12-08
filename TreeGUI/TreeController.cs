using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace TreeGUI
{
    public class TreeController
    {
        public static bool isDbg = false;
        public static bool dbgSupressAssertion = isDbg && false;
        public static bool dbgManualSerialization = isDbg && false;
        public const string mlrLinkPrefixOld = "\\\\mlr:{";
        public const string mlrLinkPrefixNew = "\\\\cfl:{";
        public const string timeFmtDateTime = "HH:mm:ss";

        public static string TimeSpanFmt(TimeSpan? span)
        {
            string timeSpanFmt = @"\:mm\:ss";
            return span?.HoursCount()+span?.ToString(timeSpanFmt);
        }

        public class Node
        {
            public bool IsExpanded { get; set; }
            private Node _parent;
            private NodeList _childs = new NodeList();

            public string label;

            //private int _index;
            [JsonProperty] internal TimePeriodList TimePeriods = new TimePeriodList();
            private TreeController _parentTree;

            public Node()
            {

            }

            public Node(Node parent)
            {
                this.Parent = parent;
                if (parent == null)
                {
                    label = "RootNode";
                }
            }

            public Node(TreeController parentTree)
            {
                _parentTree = parentTree;
            }

            public Guid Guid { get; set; } = Guid.NewGuid();

            public class NodeList : List<Node>
            {
            };

            public int Index
            {
                get { return _parent?._childs.IndexOf(this) ?? 0; }
                set
                {
                    if (Parent != null)
                    {
                        //if (!dbgSupressAssertion)
                        //Debug.Assert(Parent._childs[_index] == this);
                        Parent?._childs.Remove(this);
                        Parent?._childs.Insert(value, this);
                    }
                    else if (!TreeController._deserializing)
                        Debug.Assert(value == 0, $"Index.set({value}) without Parent");
                }
            }

            public Node Parent
            {
                get { return _parent; }
                set
                {
                    if (TreeController._deserializing)
                        _parent = value;
                    else
                    {
                        if (_parent == value)
                            return;
                        _parent?._childs.Remove(this);
                        _parent = value;
                        if (_parent != null)
                        {
                            _parent._childs.Add(this);
                        }
                    }
                }
            }


            public NodeList Childs
            {
                get { return _childs; }
            }

            public int dbg { get; set; }
            public bool Checked { get; set; }
            public string Notes { get; set; }


            private Node TryChildByLabel(string label)
            {
                return Childs.Find(node => node.label == label);
            }

            const string propertiesLabel = "_properties";

            public Node Properties
            {
                get { return TryChildByLabel(propertiesLabel); }
            }

            public void UpdateParents(Node parent)
            {
                _parent = parent;
                foreach (var argChild in Childs)
                {
                    argChild.DebugPrint($"UpdateParents");
                    argChild.UpdateParents(this);
                }
            }

            public void DebugPrint(string Text)
            {
                Debug.Print($"{Text} labels:{Parent?.label ?? ""}.{label} Idx:{Index}");
            }

            public TimeSpan TimeSpent
            {
                set
                {
                    if (value != new TimeSpan() && TimePeriods.Count == 0)
                    {
                        var period = new TimePeriod(this, new DateTime(2019, 9, 10));
                        period.Till = period.Since + value;
                        TimePeriods.Add(period);
                    }
                }
            }

            public TimeSpan TimeSpent2(DateTime sinceDate = new DateTime())
            {
                return TimePeriods.Where(tp=>tp.Since>= sinceDate).Aggregate(new TimeSpan(), (sum, next) => sum += (next.Till - next.Since)); 
            }

            public TimeSpan TimeSpentWithChilds(DateTime sinceDate=new DateTime())
            {
                return TimeSpent2(sinceDate) +
                       Childs.Aggregate(new TimeSpan(), (sum, next) => sum += next.TimeSpentWithChilds(sinceDate));
            }

            public string ParentProjectInfo(DateTime sinceDate = new DateTime())
            {
                    return
                        $"{NearestParentProject.label} :\n {TreeController.TimeSpanFmt(NearestParentProject.TimeSpentWithChilds(sinceDate))}";
            }

            public bool IsProject { get; set; }

            public TreeController.Node NearestParentProject
            {
                get { return IsProject ? this : Parent?.NearestParentProject ?? this; }

            }

            public class ViewItem
            {
                public Node Node { get; set; }
                public bool ProjectsInPath { get; set; }
                public DateTime Moment { get; set; }

                public override string ToString()
                {
                    if (ProjectsInPath)
                        return Moment.ToString(timeFmtDateTime) + " " + Node.PathWithProjects();
                    else
                        return Node.label;
                }
            }

            public string PathWithProjects(bool upward=true, string separator = "<-")
            {
                var node = this;
                var labels=new List<string>();

                labels.Add(node.label + separator);

                while (node.Parent != null)
                {
                    node = node.Parent.NearestParentProject;
                    //result = node.label + "\\" + result;
                    //result += separator + node.label;
                    labels.Add(node.label + separator);
                }

                if (!upward)
                    labels.Reverse();

                string result="";
                labels.ForEach(s=>result+=s);

                return result;
            }

            [JsonIgnore]
            public TreeController ParentTree
            {
                get { return Parent?.ParentTree ?? _parentTree; }
                set
                {
                    Debug.Assert(Parent == null);
                    _parentTree = value;
                }
            }

            public ExternalNode Extends { get; set; }

            public class ExternalNode
            {
                private Guid _treeGuid;

                [JsonIgnore]
                public Node Node
                {
                    get { return _node =_node??TreeController.TreeByGuid(TreeGuid).NodeByGuid(_guid); }
                    set
                    {
                        _node = value;
                        Guid = _node?.Guid ?? Guid.Empty;
                    }

                }

                [JsonProperty]
                private Guid TreeGuid
                {
                    get
                    {
                        return _node?.ParentTree.Guid ?? _treeGuid;
                    }

                    set { _treeGuid = value; }
                }

                private Guid _guid;
                private Node _node;

                [JsonProperty]
                private Guid Guid
                {
                    get { return TryNode?.Guid ?? _guid; }
                    set { _guid = value; }
                }

                public Node TryNode {
                    get
                    { //TODO: swap call sequence: Node->TryNode
                        try
                        {
                            return Node;
                        }
                        catch (System.InvalidOperationException)
                        {
                            return null;
                        }
                    }
                }

                public ExternalNode(Node node)
                {
                    this.Node = node;
                }

            }

            public Node.ViewItem NewViewItem(bool projectsInPath, DateTime moment)
            {
                return new ViewItem()
                {
                    Node = this,
                    ProjectsInPath = projectsInPath,
                    Moment = moment
                };
            }

            public class TimePeriod
            {
                private Node _node;

                public TimePeriod(TreeController.Node node, DateTime since)
                {
                    Node = node;
                    Since = since;
                    Till = since;
                }

                public DateTime Since { get; set; }

                public Node Node
                {
                    get { return _node; }
                    set
                    {
                        if (!TreeController._deserializing)
                            SwapItemList(_node?.TimePeriods, value?.TimePeriods, this);

                        _node = value;
                    }
                }

                private void SwapItemList<T>(List<T> srcList, List<T> destList, T item)
                {
                    Debug.Assert(item != null);
                    if (srcList == destList)
                        return;
                    srcList?.Remove(item);
                    destList?.Add(item);
                }

                public DateTime Till { get; set; }

                public override string ToString()
                {
                    return $"{Since} - {Till.ToString(timeFmtDateTime)}";
                }
            }

            public class TimePeriodList : List<TimePeriod>
            {
            };

            public Node GetOrInsertChild(Guid guid, string newLabel)
            {
                return ChildByGuid(guid) ??
                       new Node(this)
                       {
                           Guid = guid,
                           label = newLabel
                       };
            }

            public Node ChildByGuid(Guid guid)
            {
                return Childs.Find(node => node.Guid == guid);
            }

            public void SetPropertyValue(string name, DateTime value)
            {
                ForceProperties();
                var prop = Properties.GetOrInsertChildByLabel(name);
                prop.Notes = value.ToString(CultureInfo.InvariantCulture);
            }

            private void ForceProperties()
            {
                GetOrInsertChildByLabel(propertiesLabel);
            }

            private Node GetOrInsertChildByLabel(string label)
            {
                return TryChildByLabel(label) ??
                       new Node(this)
                       {
                           label = label
                       };
            }

            public DateTime GetPropertyValueDateTime(string name)
            {
                return Convert.ToDateTime(Properties?.TryChildByLabel(name)?.Notes, CultureInfo.InvariantCulture);
            }

            public TimePeriodList SelectTimePeriodsRecursive(DateTime since, DateTime till)
            {
                var nodes=Childs.SelectRecursiveList(n => n.Childs);
                nodes.Insert(0, this);
                TimePeriodList periods=new TimePeriodList();
                foreach (var node in nodes)
                {
                    periods.AddRange(node.TimePeriods.Where(p=>p.Since>since&&p.Till<till));
                }

                periods.Sort(delegate(TimePeriod a, TimePeriod b) { return a.Since < b.Since ? -1 : 1;});
                return periods;
            }
        }

        public static TreeController contr = new TreeController();
            public Node root = new Node(contr);
            public Node currentNode;
            public bool dbgRunTest = true;
            private static bool _deserializing = false;
            public event NodeEventHanler NodeAdded;

            public delegate void NodeEventHanler(Node node, bool byUser);

            public void Serialize(bool save)
            {
                var jsonPath = FilePath;
                string jsonText;
                if (save)
                {
                    jsonText = JsonConvert.SerializeObject(root,
                        new JsonSerializerSettings
                        {
                            PreserveReferencesHandling = PreserveReferencesHandling.All,
                            Formatting = Formatting.Indented
                        });
                    using (StreamWriter sw = new StreamWriter(jsonPath, false))
                    {
                        sw.WriteLine(jsonText);
                    }
                }
                else
                {
                    if (File.Exists(jsonPath))
                    {
                        using (StreamReader sr = new StreamReader(jsonPath))
                        {
                            jsonText = sr.ReadToEnd();

                            _deserializing = true;
                            try
                            {

                                ITraceWriter traceWriter = new MemoryTraceWriter();
                                root = JsonConvert.DeserializeObject<Node>(jsonText,
                                    new JsonSerializerSettings
                                    {
                                        PreserveReferencesHandling = PreserveReferencesHandling.All
                                        //,TraceWriter = traceWriter
                                    }
                                );
                                Debug.WriteLine(traceWriter);
                                root.ParentTree = this;
                            }
                            finally
                            {
                                _deserializing = false;
                            }
                        }
                    }


                    NodeAdded?.Invoke(root, false);
                    foreach (var node in root.Childs.SelectRecursive(c => c.Childs).ToArray())
                    {
                        //Debug.Print(node.label);
                        NodeAdded?.Invoke(node, false);
                    }

                    //var query =
                    //    from node in root.SelectRecursive(c => c.childs);
                    //where node.Name.Contains("5")
                    //select node.Name;
                }
            }


            public void AddNode(Node parent, bool byUser)
            {
                var node = new Node(parent);
                if (parent == null)
                    root = node;
                NodeAdded?.Invoke(node, byUser);
            }

            public void LoadFile(string path = "")
            {
                FilePath = path != "" ? path : Environment.PrepareDataPath("treeTest.json");
                contr.Serialize(false);
            }

            public string FilePath { get; set; }
        public Guid Guid
        {
            get { return root.Guid; }
        }

        public Node NodeByGuid(Guid guid)
            {
                return root.Childs.SelectRecursive(c => c.Childs).First(n => n.Guid == guid);
            }

            public void SerializeIfNeeded()
            {
                if (!dbgManualSerialization)
                    Serialize(true);
            }

            private static TreeController TreeByGuid(Guid treeGuid)
            {
                if (contr.root == null || treeGuid.Equals(contr.Guid))
                    return contr;
                if(treeGuid.Equals(System.Guid.Empty))
                    return contr;

                throw new NotImplementedException();
            }
    }

}
