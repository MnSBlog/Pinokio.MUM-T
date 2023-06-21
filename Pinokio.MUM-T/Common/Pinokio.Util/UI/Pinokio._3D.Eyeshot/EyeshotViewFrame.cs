using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using devDept.Eyeshot;
using devDept.Eyeshot.Translators;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using devDept.Graphics;

using Pinokio.Core;
namespace Pinokio._3D.Eyeshot
{
    public partial class EyeshotViewFrame : Form
    {
        private ShapeManager _shapeManager;

        private Dictionary<string, DrawSetting> _drawSettings;
        private ViewMode _mode;
        private MaterialKeyedCollection _materials;

        public ShapeManager ShapeManager { get => _shapeManager; }

        #region Selection
        private List<devDept.Eyeshot.Environment.SelectedItem> _selectedItems;
        #endregion

        #region MoveObject
        #endregion

        #region EventHandler
        public delegate void ViewFrameEventHandler(object obj);
        public event ViewFrameEventHandler ModeChange = null;
        public event ViewFrameEventHandler SelectObject = null;
        #endregion

        public EyeshotViewFrame(bool loadMaterial = false)
        {
            InitializeComponent();
            viewPort.Unlock("US2-M8WAN-12JY6-WA1Y-SRPW");
            EyeshotShapeFactory.SetEyeshotViewPort(viewPort);
            EyeshotCADMart.SetEyeshotViewPort(viewPort);

            _shapeManager = new ShapeManager();
            _drawSettings = new Dictionary<string, DrawSetting>();
            _selectedItems = new List<devDept.Eyeshot.Environment.SelectedItem>();

            InitializeViewPort(loadMaterial);
        }

        #region Initialize
        private void InitializeViewPort(bool loadMaterial)
        {
            _mode = ViewMode.TwoDim;

            viewPort.OriginSymbol.Visible = false;
            viewPort.Grid.Visible = false;
            viewPort.StopAnimation();
            viewPort.DisplayMode = displayType.Rendered;

            ModeChange += ChangeViewModeDefault;
            ModeChange(ViewMode.TwoDim);

            InitializeToolbar();
            if(loadMaterial) InitializeMaterials();
        }

        private void InitializeToolbar()
        {
            ToolBarButtonList initialToolBars = viewPort.ToolBar.Buttons;

            devDept.Eyeshot.ToolBarButton[] buttons = new devDept.Eyeshot.ToolBarButton[5];
            buttons[0] = initialToolBars[4];
            buttons[1] = initialToolBars[5];
            buttons[2] = initialToolBars[6];
            buttons[3] = new devDept.Eyeshot.ToolBarButton(null, "Separator", "", devDept.Eyeshot.ToolBarButton.styleType.Separator, true);

            Bitmap dimensionConvertBmp = Properties.Resources.DimensionConvert;
            buttons[4] = new devDept.Eyeshot.ToolBarButton(dimensionConvertBmp, "ConvertDimension", "2D/3D Change", devDept.Eyeshot.ToolBarButton.styleType.PushButton, true);

            viewPort.ToolBar.Buttons = new ToolBarButtonList(viewPort.ToolBar, buttons);
            viewPort.ToolBar.Buttons[4].Click += viewPort_DimensionChanged;
        }

        private void InitializeMaterials()
        {
            viewPort.Materials.Clear();
            _materials = new MaterialKeyedCollection();
            Action<string, string> AddMaterial = (materialName, fileName) =>
            {
                var path = System.IO.Directory.GetCurrentDirectory() + @"\Textures\" + fileName;
                Material material = new Material(materialName, new Bitmap(path));
                _materials.Add(material);
            };

            AddMaterial("Concrete", "concrete.jpg");
            AddMaterial("Cherry", "Cherry.jpg");
            AddMaterial("Bricks", "Bricks.jpg");
            AddMaterial("Maple", "Maple.jpg");
            AddMaterial("Floor", "floor_color_map.jpg");
            AddMaterial("Wenge", "Wenge.jpg");
            AddMaterial("Marble", "marble.jpg");
            AddMaterial("Display", "display.jpg");
            AddMaterial("Warning", "Warning_1.jpg");
            AddMaterial("Metal1", "metal1.jpg");
            AddMaterial("aluminium", "metal2.jpg");
            AddMaterial("Metal3", "metal3.jpg");
            AddMaterial("WhiteMetal", "white_metal.jpg");
            AddMaterial("White", "white_metal.jpg");
            AddMaterial("Blackrubber", "blackrubber.jpg");
            AddMaterial("Glass", "glass11.jpg");
            AddMaterial("Keyboard", "keyboard.jpg");
            AddMaterial("black_matte", "matblack.jpg");

            var mat = Material.Chrome;
            mat.Environment = 0.7f;
            mat.Name = "Chrome";
            _materials.Add(mat);
            mat = Material.Gold;
            mat.Environment = 0.05f;
            mat.Name = "Gold";
            _materials.Add(mat);

            foreach (Material m in _materials)
            {
                viewPort.Materials.Add(m);
            }
        }
        #endregion

        #region ViewFrame Setting
        public void ChangeViewMode(ViewMode mode)
        {
            ModeChange(mode);
            viewPort.ZoomFit();
        }
        #endregion

        public void Run()
        {
            viewPort.StartAnimation(100);
        }

        #region Add Model
        public Shape AddShape(PinokioObject pObject, EyeshotShapeType shapeType)
        {
            Shape shape = null;
            switch (shapeType)
            {
                case EyeshotShapeType.Process:
                case EyeshotShapeType.OHT:
                case EyeshotShapeType.Stocker:
                    shape = _shapeManager.AddEyeshotShape(pObject as ConcreteObject, shapeType);
                    break;
                case EyeshotShapeType.Node:
                case EyeshotShapeType.Link:
                case EyeshotShapeType.Floor:
                    shape = _shapeManager.AddEyeshotMapShape(pObject as AbstractObject, shapeType);
                    break;
            }

            return shape;
        }

        public void DrawAll()
        {
            _shapeManager.DrawAll();
            viewPort.ZoomFit();
        }

        public void DrawGrid(Point3D min, Point3D max, int step, double gap = 5000)
        {
            viewPort.Grid.Visible = true;
            viewPort.Grid.Min = min - new Point3D(gap, gap, 0);
            viewPort.Grid.Max = max + new Point3D(gap, gap, 0);
            viewPort.Grid.Step = step;
        }

        public void HideGrid()
        {
            viewPort.Grid.Visible = false;
        }
        #endregion
        private void ChangeViewModeDefault(object obj)
        {
            if (obj is ViewMode mode)
            {
                _mode = mode;
                switch (_mode)
                {
                    case ViewMode.TwoDim:
                        viewPort.CoordinateSystemIcon.Visible = false;
                        viewPort.SetView(viewType.Top);
                        viewPort.ViewCubeIcon.Visible = false;
                        viewPort.Camera.ProjectionMode = projectionType.Orthographic;
                        viewPort.Rotate.Enabled = false;
                        break;
                    case ViewMode.ThreeDim:
                        viewPort.CoordinateSystemIcon.Visible = true;
                        viewPort.SetView(viewType.Trimetric);
                        viewPort.ViewCubeIcon.Visible = true;
                        viewPort.Camera.ProjectionMode = projectionType.Perspective;
                        viewPort.Rotate.Enabled = true;
                        break;
                }

                // Refresh View
                viewPort.Entities.Regen();
                viewPort.Invalidate();
            }
        }

        #region Event Handles
        private void viewPort_DimensionChanged(object sender, EventArgs e)
        {
            switch (_mode)
            {
                case ViewMode.TwoDim:
                    ModeChange(ViewMode.ThreeDim);
                    break;
                case ViewMode.ThreeDim:
                    ModeChange(ViewMode.TwoDim);
                    break;
            }
            viewPort.ZoomFit();
        }

        private void viewPort_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int entityIndex = viewPort.GetEntityUnderMouseCursor(e.Location);
                if (entityIndex > -1)
                {
                    var entity = viewPort.Entities[entityIndex];
                    if (entity.EntityData != null && entity.EntityData is Shape shape && shape.Core != null)
                    {
                        viewPort.Entities.ClearSelection();
                        var selItem = new devDept.Eyeshot.Environment.SelectedItem(new Stack<BlockReference>(), entity);
                        // Selects the item
                        selItem.Select(viewPort, true);
                        if (SelectObject != null)
                        {
                            SelectObject(shape.Core);
                            viewPort.Invalidate();
                        }

                    }
                }
            }
            //if (e.Button != MouseButtons.Right || viewPort.ActionMode != actionType.None || viewPort.ToolBar.Contains(e.Location))
            //{
            //    Stack<devDept.Eyeshot.Entities.BlockReference> parents = new Stack<devDept.Eyeshot.Entities.BlockReference>();
            //    _entityIndex = viewPort.GetEntityUnderMouseCursor(e.Location) + 1;
            //    var br = (BlockReference)viewPort.Entities[_entityIndex];

            //    //if(_)
            //    if (ShapeManager.Instance.MeshComponent.ContainsKey(Convert.ToUInt32(_entityIndex)))
            //    {
            //        List<Entity> entities = ShapeManager.Instance.MeshComponent[Convert.ToUInt32(_entityIndex)].Entities;

            //        viewPort.Entities.ClearSelection();
            //        foreach (Entity entity in entities)
            //        {
            //            // The top most parent is the root Blockreference: must reverse the order, creating a new Stack
            //            var selItem = new devDept.Eyeshot.Environment.SelectedItem(new Stack<BlockReference>(parents), entity);

            //            // Selects the item
            //            selItem.Select(viewPort, true);
            //        }
            //        viewPort.Invalidate();
            //    }
            //    _parent.UcProperty.RefreshModelInfo(Convert.ToUInt32(_entityIndex));
            //}
            //else
            //{
            //    // TK 
            //    // gets the entity index
            //    _entityIndex = viewPort.GetEntityUnderMouseCursor(e.Location);
            //    if (_entityIndex < 0)
            //        return;
            //    if (ShapeManager.Instance.ModelShapes.ContainsKey(Convert.ToUInt32(_entityIndex + 1)))
            //    {
            //        if (ShapeManager.Instance.ModelShapes[Convert.ToUInt32(_entityIndex + 1)].CoreNode.Type is NODE_TYPE.AGV)
            //        {
            //            // gets 3D start point
            //            //viewPort.ScreenToPlane(e.Location, _plane, out _moveFrom);                        

            //            _selectedAGV = ModelManager.Instance.Models[Convert.ToUInt32(_entityIndex + 1)] as AGV;
            //            _moveFrom = new Point3D(_selectedAGV.PosVec3.X * 0.001, _selectedAGV.PosVec3.Y * 0.001, _selectedAGV.PosVec3.Z * 0.001);


            //            // Edge 색 강조
            //            if (_selectedAGV.Path.Count > 0)
            //            {
            //                foreach (var link in _selectedAGV.Path)
            //                {
            //                    var shape = ShapeManager.Instance.EdgeShapes[link];
            //                    var mesh = shape.Entity;
            //                    mesh.Translate(0, 0, 0.02);
            //                    mesh.ColorMethod = colorMethodType.byEntity;

            //                    if (_selectedAGV.Path.IndexOf(link) == 0)
            //                        mesh.Color = Color.Green;
            //                    else if (_selectedAGV.Path.IndexOf(link) == _selectedAGV.Path.Count - 1)
            //                        mesh.Color = Color.Blue;
            //                    else
            //                        mesh.Color = Color.Red;
            //                }
            //            }
            //            viewPort.Entities.Regen();
            //            viewPort.Invalidate();
            //        }
            //    }
            //}
        }

        private void viewPort_MouseUp(object sender, MouseEventArgs e)
        {
            // selected 
            //if (ShapeManager.Instance.ModelShapes.ContainsKey(Convert.ToUInt32(_entityIndex + 1)) && e.Button != MouseButtons.Left)
            //{
            //    if (ShapeManager.Instance.ModelShapes[Convert.ToUInt32(_entityIndex + 1)].CoreNode.Type is NODE_TYPE.AGV)
            //    {
            //        #region AGV 위치 옮기기
            //        Entity entity = viewPort.Entities[_entityIndex] as Entity;
            //        Point3D moveTo;

            //        //마우스 위치로 옮기고 싶으면 이거
            //        //viewPort.ScreenToPlane(e.Location, _plane, out moveTo);
            //        //Vector3D delta = Vector3D.Subtract(moveTo, _moveFrom);

            //        if (_selectedAGV is null) return;

            //        #region 가까운 노드 찾기
            //        Point3D _current;
            //        viewPort.ScreenToPlane(e.Location, _plane, out _current);

            //        Vector3 currentPoint = new Vector3(_current.X * 1000, _current.Y * 1000, _current.Z);
            //        Vertex closestNode = null;
            //        double minimum_Distance = 99999;
            //        List<Vertex> nodes = new List<Vertex>();

            //        if (_selectedAGV.Path.Count > 0)
            //        {
            //            nodes.Add(Graph.Instance.Links[_selectedAGV.Path[0]].FromNode);
            //            for (int i = 0; i < _selectedAGV.Path.Count; i++)
            //            {
            //                nodes.Add(Graph.Instance.Links[_selectedAGV.Path[i]].ToNode);
            //            }
            //        }

            //        foreach (var node in Graph.Instance.Nodes)
            //        {
            //            double distance = Math.Sqrt(Math.Pow(node.Value.PosVec3.X - currentPoint.X, 2) + Math.Pow(node.Value.PosVec3.Y - currentPoint.Y, 2));
            //            if (minimum_Distance > distance)
            //            {
            //                closestNode = node.Value;
            //                minimum_Distance = distance;
            //            }

            //            if (minimum_Distance == distance)
            //            {
            //                if (nodes.Contains(node.Value))
            //                    closestNode = node.Value;
            //            }
            //        }

            //        //가장 가까운 위치로 옮기고 싶으면 이거
            //        //moveTo = Converter.ToVec3D(posVec3) * (0.001);
            //        #endregion

            //        //Vector3D delta = Vector3D.Subtract(new Point3D(closestNode.PosVec3.X * 0.001, closestNode.PosVec3.Y * 0.001, 0), _moveFrom);
            //        // applies the translation
            //        //entity.Translate(delta);

            //        // regens entities that need it
            //        viewPort.Entities.Regen();

            //        // refresh the screen
            //        viewPort.Invalidate();

            //        // sets start as current
            //        _moveFrom = new Point3D(closestNode.PosVec3.X * 0.001, closestNode.PosVec3.Y * 0.001, 0);
            //        #endregion

            //        if (_selectedAGV.Path.Count > 0 && closestNode != null)
            //        {
            //            if (nodes.Contains(closestNode))
            //            {
            //                for (int i = 0; i < _selectedAGV.Path.Count; i++)
            //                {
            //                    if (Graph.Instance.Links[_selectedAGV.Path[i]].FromNode == closestNode)
            //                    {
            //                        var currentLink = Graph.Instance.Links[_selectedAGV.Path[i]];
            //                        _selectedAGV.Location = new Location() { Link = currentLink, Offset = 0 };
            //                        _selectedAGV.Speed = 0;
            //                        _selectedAGV.CalculatePosition();
            //                        ShapeManager.Instance.ModelShapes[Convert.ToUInt32(_entityIndex + 1)].Pos = Converter.ToVec3D(_selectedAGV.PosVec3) * (0.001);

            //                        break;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                foreach (var outLink in closestNode.OutLinks)
            //                {
            //                    if (outLink.LinkType == LINK_TYPE.STRAIGHT)
            //                    {
            //                        _selectedAGV.CurrentLink = outLink;
            //                        break;
            //                    }
            //                }
            //                //_selectedAGV.CurrentLink = Network.Instance.Links[_selectedAGV.Path[i]];
            //                _selectedAGV.CalculatePosition();
            //                _selectedAGV.Speed = 0;
            //                //_selectedAGV.SendPacket("Forgot Location");
            //            }


            //            // Edge 색 초기화
            //            foreach (var link in _selectedAGV.Path)
            //            {
            //                var mesh = ShapeManager.Instance.EdgeShapes[link].Entity;
            //                mesh.Translate(0, 0, -0.02);
            //                mesh.ColorMethod = colorMethodType.byEntity;
            //                mesh.Color = Color.Yellow;
            //            }
            //            //_selectedAGV.AdjustCount++;
            //        }

            //        _entityIndex = -1;
            //        _selectedAGV = null;
            //        viewPort.Invalidate();
            //    }
            //}
        }

        private void viewPort_MouseLeave(object sender, EventArgs e)
        {
            //Log.Info("Mouse Leave");
        }

        private void viewPort_ErrorOccurred(object sender, devDept.Eyeshot.Environment.ErrorOccurredEventArgs args)
        {
            LogHandler.AddLog(LogLevel.Error, args.StackTrace + " ; " + args.Message);
        }

        private void viewPort_DoubleClick(object sender, EventArgs e)
        {
            var me = (MouseEventArgs)e;
            int entityIndex = viewPort.GetEntityUnderMouseCursor(me.Location);
            if (entityIndex > 0)
            {
                var entity = viewPort.Entities[entityIndex];
                if (entity.EntityData is EyeshotShape eShape)
                {
                    if (eShape.Core != null)
                    {
                        LogHandler.AddLog(LogLevel.Info, $"{eShape.Core.Id} / {eShape.Core.Name}");
                    }
                }
            }
        }

        private void viewPort_SelectionChanged(object sender, devDept.Eyeshot.Environment.SelectionChangedEventArgs e)
        {
            // updates the selection data
            if (_selectedItems == null)
            {
                _selectedItems = new List<devDept.Eyeshot.Environment.SelectedItem>();
                _selectedItems.AddRange(e.AddedItems);
            }
            else
            {
                foreach (devDept.Eyeshot.Environment.SelectedItem item in e.RemovedItems)
                {
                    _selectedItems.Remove(item);
                }

                _selectedItems.AddRange(e.AddedItems);
            }
        }
        #endregion
    }
}
