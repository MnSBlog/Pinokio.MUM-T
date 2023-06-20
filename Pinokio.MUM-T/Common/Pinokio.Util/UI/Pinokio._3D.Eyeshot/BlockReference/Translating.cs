using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;

namespace Pinokio._3D.Eyeshot
{
    // Animation Class for Translating(Move Position & Change Direction of the Model)
    public class Translating : BlockReference
    {
        private Transformation _customTransform;
        public EyeshotShape ThisShape { get => (EyeshotShape)this.EntityData; }
        public Translating(string blockName)
            : base(blockName)
        { }

        public override void MoveTo(DrawParams data)
        {
            try
            { 
                base.MoveTo(data);

                _customTransform = new Translation(EyeshotHelper.ToVector3D(ThisShape.Pos));
                data.RenderContext.MultMatrixModelView(_customTransform);

                _customTransform = new Rotation(ThisShape.Radian, Vector3D.AxisZ);
                data.RenderContext.MultMatrixModelView(_customTransform);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
