using System;
using System.Collections.Generic;
using System.Drawing;

using Pinokio.Core;

using devDept.Geometry;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;

namespace Pinokio._3D.Eyeshot
{
    public class EProcessShape : EyeshotShape
    {
        public EProcessShape(uint id, ConcreteObject conObj, ViewPort viewPort) : base(id, conObj, viewPort)
        { }

        //protected override List<Entity> DrawByShape()
        //{
        //    var entities = new List<Entity>();
        //    Mesh bodyMain = EyeshotCADMart.CreateBox(1000, 814.81, 765.96, "WhiteMetal", new Vector3D(0, 185.19, 21.28));
        //    entities.Add(bodyMain);

        //    Mesh bodyBottom = EyeshotCADMart.CreateBox(1000, 814.81, 21.27, "Metal3", new Vector3D(0, 185.19, 0));
        //    entities.Add(bodyBottom);

        //    Mesh bodyTop = EyeshotCADMart.CreateBox(1000, 814.81, 42.55, "Blackrubber", new Vector3D(0, 185.19, 787.23));
        //    entities.Add(bodyTop);

        //    Mesh alarmBase = EyeshotCADMart.CreateCylinder(18.51, 42.55, Color.DarkGray, new Vector3D(46.51, 259.26, 829.79));
        //    entities.Add(alarmBase);

        //    Mesh alarmGreen = EyeshotCADMart.CreateCylinder(18.51, 42.55, Color.Green, new Vector3D(46.51, 259.26, 872.34));
        //    entities.Add(alarmGreen);

        //    Mesh alarmOrange = EyeshotCADMart.CreateCylinder(18.51, 42.55, Color.DarkOrange, new Vector3D(46.51, 259.26, 914.89));
        //    entities.Add(alarmOrange);

        //    Mesh alarmRed = EyeshotCADMart.CreateCylinder(18.51, 42.55, Color.Red, new Vector3D(46.51, 259.26, 957.45));
        //    entities.Add(alarmRed);

        //    Mesh boxDisplay = EyeshotCADMart.CreateBox(186, 18.51, 17, "Display", new Vector3D(790.7, 181.48, 553.19));
        //    entities.Add(boxDisplay);

        //    Mesh boxGlass = EyeshotCADMart.CreateBox(534.88, 18.51, 425.53, "Glass", new Vector3D(116.28, 181.48, 297.87));
        //    entities.Add(boxGlass);

        //    Mesh keybox = EyeshotCADMart.CreateBox(186, 185.18, 42.55, "WhiteMetal", new Vector3D(790.7, 0, 340.42));
        //    entities.Add(keybox);

        //    Mesh keyboard = EyeshotCADMart.CreateBox(139.53, 111.11, 42.55, "Keyboard", new Vector3D(813.95, 18.52, 382.98));
        //    entities.Add(keyboard);

        //    var size = DrawSetting.Size * 0.001; // 기본 도형의 사이즈가 1000x1000x1000이므로 0.001을 곱해 Scale한다.
        //    foreach (var ent in entities)
        //    {
        //        ent.Scale(EyeshotHelper.ToVector3D(size));
        //        ent.Translate(-DrawSetting.Width / 2, -DrawSetting.Depth / 2);
        //    } 

        //    return entities;
        //}

        protected override List<Entity> DrawByBlock()
        {
            var eDrawSetting = DrawSetting as EyeshotDrawSetting;
            var blockRef = new BlockReference(eDrawSetting.BlockName);
            blockRef.ColorMethod = colorMethodType.byEntity;
            blockRef.Color = DrawSetting.MainColor;


            blockRef.Translate(-DrawSetting.Width / 2, -DrawSetting.Depth / 2);

            return new List<Entity> { blockRef };
        }

        public override void Draw()
        {
            base.Draw();
            //var label = new devDept.Eyeshot.Labels.LeaderAndText(EyeshotHelper.ToPoint3D(this.Pos),
            //    this.Core.Name,
            //    new System.Drawing.Font("맑은고딕", 8),
            //    System.Drawing.Color.Black,
            //    new Vector2D(10, 10));

            //var label = new devDept.Eyeshot.Labels.TextOnly(EyeshotHelper.ToPoint3D(this.Pos),
            //    this.Core.Name,
            //    new System.Drawing.Font("맑은고딕", 8),
            //    System.Drawing.Color.Black);
            //ViewPort.Labels.Add(label);
        }

        protected override List<Entity> DrawByShape()
        {
            List<Entity> entities = new List<Entity>();
            if (!DrawSetting.DrawByFile)
            {
                Mesh bodyMain = EyeshotCADMart.CreateBox(DrawSetting.Width, DrawSetting.Depth, 1000, DrawSetting.MainColor, new Vector3D(0, 0, 0));
                bodyMain.ColorMethod = colorMethodType.byParent;
                entities.Add(bodyMain);

                //foreach (var ent in entities)
                //{
                //    ent.Translate(-DrawSetting.Width / 2, -DrawSetting.Depth / 2);
                //}
            }
            else
            {
                entities = EyeshotCADMart.GetCADByName(DrawSetting.FileName);
                foreach (var ent in entities)
                {
                    ent.Scale(EyeshotHelper.ToVector3D(DrawSetting.Size));
                    ent.Translate(DrawSetting.Width / 2, DrawSetting.Depth / 2);
                }
            }
            return entities;
        }

        private string _lastLevel = "None";
        public override void UpdateColor(object obj)
        {
            string level = (string)obj;
            if (_lastLevel != level)
            {
                Color color = Color.White;
                switch (level)
                {
                    case "None":
                        color = DrawSetting.MainColor;
                        break;
                    case "Low":
                        color = EquipmentColors.Low;
                        break;
                    case "Medium":
                        color = EquipmentColors.Medium;
                        break;
                    case "High":
                        color = EquipmentColors.High;
                        break;
                }

                foreach (var ent in this.Entities)
                {
                    ent.Color = color;
                }
                _lastLevel = level;
            }
        }
    }
}
