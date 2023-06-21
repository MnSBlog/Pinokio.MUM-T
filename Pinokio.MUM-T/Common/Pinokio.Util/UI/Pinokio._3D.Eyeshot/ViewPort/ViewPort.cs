using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using devDept.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Pinokio._3D.Eyeshot
{ 
    public partial class ViewPort : devDept.Eyeshot.Model
    {
        private bool firstClick = true;

        // Active layer index
        public string ActiveLayerName;

        // Always draw on XY plane, view is alwyas topview
        private Plane plane = Plane.XY;

        // Current selection/position
        private Point3D current;

        // List of selected or picked points with left mouse button 
        private List<Point3D> points = new List<Point3D>();

        public List<Entity> selEntities = new List<Entity>();

        // Current mouse position
        private System.Drawing.Point mouseLocation;

        // Selected entity, store on LMB click
        private int selEntityIndex;
        private Entity selEntity = null;

        // Current drawing plane and extension points required while dimensioning
        private Plane drawingPlane;
        private Point3D extPt1;
        private Point3D extPt2;

        // Current arc radius
        private double radius, radiusY;
        // Current arc span angle
        private double arcSpanAngle;

        // Entities for angularDim with lines
        public Line firstLine = null;
        public Line secondLine = null;
        public Point3D quadrantPoint = null;

        //Threshold to unerstand if polyline or curve has to be closed or not
        private const int magnetRange = 3;

        //Label to show wich operation is currently selected
        private String activeOperationLabel = "";

        //label to show how to exit from a command (visibile just in case an operation is currently selected)
        private string rmb = "  RMB to exit.";

        public static Color DrawingColor = Color.Black;

        private bool cursorOutside;

        //public SHAPE_TYPE drawType { get; set; }
        private bool _linkConn = false;

        public ViewPort()
        {
        }

        //public void AddToGeneratingShape(Shape shape)
        //{
        //    lock (_genShapes)
        //    {
        //        _genShapes.Add(shape);
        //    }
        //}

        //public void AddToDeletingShape(Shape shape)
        //{
        //    lock (_delShapes)
        //    {
        //        _delShapes.Add(shape);
        //    }
        //}

        public void UpdateAni()
        {
            OnAnimationTimerTick(null);
        }

        //protected override void OnAnimationTimerTick(object stateInfo)
        //{
        //    BeginInvoke(new Action(() =>
        //    {
        //        lock (_genShapes) lock (this.Entities)
        //            {
        //                for (int i = 0; i < _genShapes.Count; i++)
        //                {
        //                    var shape = _genShapes[i];
        //                    shape.Draw();
        //                    var layerName = _shapeManager.GetNodeLayerName(shape.Pos.Z, shape.Type);
        //                    this.Entities.Add(shape.BlockRef, layerName);
        //                }
        //                _genShapes.Clear();
        //            }

        //        lock (_delShapes) lock (this.Blocks) lock (this.Entities)
        //                {
        //                    for (int i = 0; i < _delShapes.Count; i++)
        //                    {
        //                        var shape = _delShapes[i];
        //                        this.Entities.Remove(shape.BlockRef);
        //                        this.Blocks.Remove(shape.GetBlock);
        //                        if (shape.Label != null)
        //                        {
        //                            lock (Labels)
        //                            { Labels.Remove(shape.Label); }
        //                        }
        //                    }
        //                    _delShapes.Clear();
        //                }

        //        base.OnAnimationTimerTick(stateInfo);
        //    }));
        //}

        protected override void OnMouseDown(MouseEventArgs e)
        {
            //this.selEntityIndex = GetEntityUnderMouseCursor(e.Location);
            //Cursor.Show();

            if (ToolBar.Contains(e.Location))
            {
                base.OnMouseDown(e);

                return;
            }
            #region Handle LMB Clicks 
            //if (ActionMode == actionType.None && e.Button == MouseButtons.Left)
            //{
            //    // we need to skip adding points for entity selection click
            //    editingMode = doingOffset || doingMirror || doingExtend || doingTrim || doingFillet || doingChamfer || doingTangents;

            //    ScreenToPlane(e.Location, plane, out current);

            //    if (IsPolygonClosed())//control needed to close curve and polyline when cursor is near the starting point of polyline or curve
            //    {
            //        //if the distance from current point and first point stored is less than given threshold
            //        points.Add((Point3D)points[0].Clone()); //the point to add to points is the first point stored.
            //        current = (Point3D)points[0].Clone();
            //    }
            //    else
            //    {
            //        if (!(editingMode && firstClick))
            //            points.Add(current);
            //    }
            //    firstClick = false;

            //    // If drawing points, create and add new point entity on each LMB click

            //    // If LINE drawing is finished, create and add line entity to model
            //    if (drawingLine && points.Count == 2)
            //    {
            //        Line line = new Line(points[0], points[1]);

            //        if (drawType == SHAPE_TYPE.LINK)
            //            AddAndRefresh(line, Color.Gray);
            //        else if (drawType == SHAPE_TYPE.MAP_LINK)
            //            AddAndRefresh(line, Color.Black);

            //        drawingLine = false;
            //    }
            //    // If CIRCLE drawing is finished, create and add a circle entity to model
            //    else if (drawingCircle && points.Count == 2)
            //    {
            //        //Circle circle = new Circle(drawingPlane, drawingPlane.Origin, radius);
            //        devDept.Eyeshot.Entities.Region circleRegion = devDept.Eyeshot.Entities.Region.CreateCircle(drawingPlane, drawingPlane.Origin, radius);

            //        if (drawType == SHAPE_TYPE.COMMIT)
            //            AddAndRefresh(circleRegion, Color.Blue);
            //        else if (drawType == SHAPE_TYPE.COMPLETE)
            //            AddAndRefresh(circleRegion, Color.Red);
            //        else if (drawType == SHAPE_TYPE.MAP_LINK)
            //            AddAndRefresh(circleRegion, Color.Black);

            //        drawingCircle = false;
            //    }

            //    else if (drawingRectangle && points.Count == 2)
            //    {
            //        //CompositeCurve rect = CompositeCurve.CreateRectangle(points[0].X, points[0].Y, current.X - points[0].X, current.Y - points[0].Y);
            //        RectangularRegion rectRegion = new RectangularRegion(points[0].X, points[0].Y, current.X - points[0].X, current.Y - points[0].Y);

            //        if (drawType == SHAPE_TYPE.BUFFER)
            //            AddAndRefresh(rectRegion, Color.DarkGray);
            //        else if (drawType == SHAPE_TYPE.PROCESS)
            //            AddAndRefresh(rectRegion, Color.Green);
            //        else if (drawType == SHAPE_TYPE.OHT)
            //            AddAndRefresh(rectRegion, Color.Yellow);

            //        drawingRectangle = false;
            //    }

            //    // If ARC drawing is finished, create and add an arc entity to model
            //    // Input - Center and two end points
            //    else if (drawingArc && points.Count == 3)
            //    {
            //        Arc arc = new Arc(drawingPlane, drawingPlane.Origin, radius, 0, arcSpanAngle);
            //        AddAndRefresh(arc, ActiveLayerName);

            //        drawingArc = false;
            //    }
            //    // If drawing ellipse, create and add ellipse entity to model
            //    // Inputs - Ellipse center, End of first axis, End of second axis
            //    else if (drawingEllipse && points.Count == 3)
            //    {
            //        Ellipse ellipse = new Ellipse(drawingPlane, drawingPlane.Origin, radius, radiusY);
            //        AddAndRefresh(ellipse, ActiveLayerName);

            //        drawingEllipse = false;
            //    }
            //    // If EllipticalArc drawing is finished, create and add EllipticalArc entity to model
            //    // Input - Ellipse center, End of first axis, End of second axis, end point
            //    else if (drawingEllipticalArc && points.Count == 4)
            //    {
            //        EllipticalArc ellipticalArc = new EllipticalArc(drawingPlane, drawingPlane.Origin, radius, radiusY, 0, arcSpanAngle, true);
            //        AddAndRefresh(ellipticalArc, ActiveLayerName);

            //        drawingEllipticalArc = false;
            //    }
            //    else if (doingExtend && firstSelectedEntity != null && secondSelectedEntity != null)
            //    {
            //        ExtendEntity();
            //        ClearAllPreviousCommandData();
            //    }
            //    else if (doingTrim && firstSelectedEntity != null && secondSelectedEntity != null)
            //    {
            //        TrimEntity();
            //        ClearAllPreviousCommandData();
            //    }
            //    else if (doingFillet && firstSelectedEntity != null && secondSelectedEntity != null)
            //    {
            //        CreateFilletEntity();
            //        ClearAllPreviousCommandData();
            //    }
            //    else if (doingChamfer && firstSelectedEntity != null && secondSelectedEntity != null)
            //    {
            //        CreateChamferEntity();
            //        ClearAllPreviousCommandData();
            //    }
            //    else if (doingMove && points.Count == 2)
            //    {
            //        if (points.Count == 2)
            //        {
            //            foreach (Entity ent in this.selEntities)
            //            {
            //                Vector3D movement = new Vector3D(points[0], points[1]);
            //                ent.Translate(movement);
            //            }

            //            Entities.Regen();
            //            ClearAllPreviousCommandData();
            //        }
            //    }
            //    else if (doingRotate)
            //    {
            //        if (points.Count == 3)
            //        {
            //            foreach (Entity ent in this.selEntities)
            //            {
            //                ent.Rotate(arcSpanAngle, Vector3D.AxisZ, points[0]);
            //            }

            //            Entities.Regen();
            //            ClearAllPreviousCommandData();
            //        }
            //    }
            //    else if (doingScale)
            //    {
            //        if (points.Count == 3)
            //        {
            //            foreach (Entity ent in this.selEntities)
            //            {
            //                ent.Scale(points[0], scaleFactor);
            //            }

            //            Entities.Regen();
            //            ClearAllPreviousCommandData();
            //        }
            //    }
            //}
            #endregion

            #region Handle RMB Clicks
//            else if (e.Button == MouseButtons.Right)
//            {
//                ScreenToPlane(e.Location, plane, out current);

//                if (drawingPoints)
//                {
//                    points.Clear();
//                    drawingPoints = false;
//                }
//                else if (drawingText)
//                {
//                    drawingText = false;
//                }
//                else if (drawingLeader)
//                {
//                    drawingLeader = false;
//                }
//                // If drawing polyline, create and add LinearPath entity to model
//                //else if (drawingPolyLine)
//                //{
//                //    LinearPath lp = new LinearPath(points);
//                //    AddAndRefresh(lp, ActiveLayerName);

//                //    drawingPolyLine = false;
//                //}
//                // If drawing spline, create and add curve entity to model
//                else if (drawingCurve)
//                {
//#if NURBS
//                    Curve curve = Curve.CubicSplineInterpolation(points);
//                    AddAndRefresh(curve, ActiveLayerName);
//#endif
//                    drawingCurve = false;
//                }
//                else
//                {
//                    ClearAllPreviousCommandData();
//                }
//            }
            #endregion

            #region Handle MMB Clicks
            if(e.Button == MouseButtons.Middle)
            {
                base.OnKeyDown(new KeyEventArgs(Keys.Control));
            }
            #endregion
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            cursorOutside = true;
            base.OnMouseLeave(e);

            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            cursorOutside = false;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // save the current mouse position
            //mouseLocation = e.Location;

            //if (ActionMode == actionType.SelectVisibleByPickDynamic)
            //{
            //    this.selEntities.Clear();

            //    for (int i = this.Entities.Count - 1; i > -1; i--)
            //    {
            //        Entity ent = this.Entities[i];
            //        if (ent.Selected && (ent is ICurve || ent is BlockReference || ent is Text || ent is Leader) ||
            //            ent is devDept.Eyeshot.Entities.Region)
            //        {
            //            this.selEntities.Add(ent);
            //        }
            //    }

            //    if (this.selEntities.Count == 0)
            //        return;

            //    ClearPreviousSelection();
            //    this.doingMove = true;
            //    foreach (Entity curve in this.selEntities)
            //        curve.Selected = true;
            //    Cursor.Hide();
            //}

            //if (drawingLine == true)
            //{
            //    int idEnt = this.GetEntityUnderMouseCursor(mouseLocation, false);
            //    if (idEnt != -1)
            //    {
            //        Entity ent = this.Entities[idEnt] as Entity;
            //        ent.Selected = true;
            //    }
            //    else
            //        this.Entities.ClearSelection();

            //    this.Invalidate();
            //}

            //// if start is valid and actionMode is None and it's not in the toolbar area
            //if (current == null || ActionMode != actionType.None || ToolBar.Contains(mouseLocation))
            //{
            //    base.OnMouseMove(e);

            //    return;
            //}

            //// paint the viewport surface
            //PaintBackBuffer();

            //// consolidates the drawing
            //SwapBuffers();

            //if (drawingPoints)
            //    activeOperationLabel = "Points: ";
            //else if (drawingText)
            //    activeOperationLabel = "Text: ";
            //else if (drawingLeader)
            //    activeOperationLabel = "Leader: ";
            //else if (drawingLine)
            //    activeOperationLabel = "Line: ";
            //else if (drawingEllipse)
            //    activeOperationLabel = "Ellipse: ";
            //else if (drawingEllipticalArc)
            //    activeOperationLabel = "EllipticalArc: ";
            //else if (drawingCircle)
            //    activeOperationLabel = "Circle: ";
            //else if (drawingRectangle)
            //    activeOperationLabel = "Rectangle: ";
            //else if (drawingArc)
            //    activeOperationLabel = "Arc: ";
            //else if (drawingPolyLine)
            //    activeOperationLabel = "Polyline: ";
            //else if (drawingCurve)
            //    activeOperationLabel = "Spline: ";
            //else if (doingMirror)
            //    activeOperationLabel = "Mirror: ";
            //else if (doingOffset)
            //    activeOperationLabel = "Offset: ";
            //else if (doingTrim)
            //    activeOperationLabel = "Trim: ";
            //else if (doingExtend)
            //    activeOperationLabel = "Extend: ";
            //else if (doingFillet)
            //    activeOperationLabel = "Fillet: ";
            //else if (doingChamfer)
            //    activeOperationLabel = "Chamfer: ";
            //else if (doingTangents)
            //    activeOperationLabel = "Tangents: ";
            //else if (doingMove)
            //    activeOperationLabel = "Move: ";
            //else if (doingRotate)
            //    activeOperationLabel = "Rotate: ";
            //else if (doingScale)
            //    activeOperationLabel = "Scale: ";
            //else
            //    activeOperationLabel = "";
            base.OnMouseMove(e);

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Middle)
            {
                base.OnKeyUp(new KeyEventArgs(Keys.Control));
            }
            base.OnMouseUp(e);
        }

        protected override void DrawOverlay(Model.DrawSceneParams myParams)
        {
            ScreenToPlane(mouseLocation, plane, out current);
            // set GL for interactive draw or elastic line 
            renderContext.SetLineSize(1);

            renderContext.EnableXOR(true);

            renderContext.SetState(depthStencilStateType.DepthTestOff);

            if (drawingLine || drawingPolyLine)
            {
                DrawInteractiveLines();
            }
            else if (drawingCircle && points.Count > 0)
            {
                if (ActionMode == actionType.None && !ToolBar.Contains(mouseLocation))
                {
                    DrawInteractiveCircle();
                }
            }
            else if (drawingRectangle && points.Count > 0)
            {
                if (ActionMode == actionType.None && !ToolBar.Contains(mouseLocation))
                {
                    DrawInteractiveRectangle();
                }
            }
            else if (drawingArc && points.Count > 0)
            {
                if (ActionMode == actionType.None && !ToolBar.Contains(mouseLocation))
                {
                    DrawInteractiveArc();
                }
            }
            else if (drawingEllipse && points.Count > 0)
            {
                DrawInteractiveEllipse();
            }
            else if (drawingEllipticalArc && points.Count > 0)
            {
                DrawInteractiveEllipticalArc();
            }
            else if (drawingCurve)
            {
                DrawInteractiveCurve();
            }
            else if (drawingLeader)
            {
                DrawInteractiveLeader();
            }
            else if (doingMove)
            {
                MoveEntity();
            }
            else if (doingScale)
            {
                ScaleEntity();
            }
            else if (doingRotate)
            {
                RotateEntity();
            }

            // disables draw inverted
            renderContext.EnableXOR(false);
            base.DrawOverlay(myParams);
        }

        internal void ClearAllPreviousCommandData()
        {
            points.Clear();
            selEntity = null;
            selEntityIndex = -1;
            drawingArc = false;
            drawingCircle = false;
            drawingRectangle = false;
            drawingCurve = false;
            drawingEllipse = false;
            drawingEllipticalArc = false;
            drawingLine = false;
            drawingPoints = false;
            drawingText = false;
            drawingLeader = false;
            drawingPolyLine = false;

            firstClick = true;
            doingMirror = false;
            doingOffset = false;
            doingTrim = false;
            doingExtend = false;
            doingChamfer = false;
            doingMove = false;
            doingScale = false;
            doingRotate = false;
            doingFillet = false;
            doingTangents = false;

            _linkConn = false;

            firstSelectedEntity = null;
            secondSelectedEntity = null;

            firstLine = null;
            secondLine = null;
            quadrantPoint = null;

            activeOperationLabel = "";
            ActionMode = actionType.None;
            Entities.ClearSelection();
            ObjectManipulator.Cancel();
        }

        private void ClearPreviousSelection()
        {
            SetView(viewType.Top, false, true);
            ClearAllPreviousCommandData();
        }

        /// <summary>
        /// Represents a vertex type from model like center, mid point, etc.
        /// </summary>
        public enum objectSnapType
        {
            None,
            Point,
            End,
            Mid,
            Center,
            Quad
        }
    }
}
