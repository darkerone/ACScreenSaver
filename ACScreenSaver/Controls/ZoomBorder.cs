using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ACScreenSaver.Controls
{
    public class ZoomBorder : Border
    {
        private UIElement child = null;
        private Point origin;
        private Point start;
        private const int exifOrientationID = 0x112; //274

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            TransformGroup transformGroup = (TransformGroup)element.RenderTransform;
            TransformCollection transformCollection = transformGroup.Children;
            TranslateTransform translateTransform = (TranslateTransform)transformCollection.First(tr => tr is TranslateTransform);
            return translateTransform;
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            TransformGroup transformGroup = (TransformGroup)element.RenderTransform;
            TransformCollection transformCollection = transformGroup.Children;
            ScaleTransform scaleTransform = (ScaleTransform)transformCollection.First(tr => tr is ScaleTransform);
            return scaleTransform;
        }

        private RotateTransform GetRotateTransform(UIElement element)
        {
            TransformGroup transformGroup = (TransformGroup)element.RenderTransform;
            TransformCollection transformCollection = transformGroup.Children;
            RotateTransform rotateTransform = (RotateTransform)transformCollection.First(tr => tr is RotateTransform);
            return rotateTransform;
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            this.child = element;
            if (child != null)
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                RotateTransform rt = new RotateTransform();
                group.Children.Add(rt);
                child.RenderTransform = group;
                child.RenderTransformOrigin = new Point(0.0, 0.0);
                this.MouseWheel += child_MouseWheel;
                //this.MouseLeftButtonDown += child_MouseLeftButtonDown;
                //this.MouseLeftButtonUp += child_MouseLeftButtonUp;
                this.MouseMove += child_MouseMove;
                this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(
                  child_PreviewMouseRightButtonDown);
            }
        }

        public void Reset()
        {
            if (child != null)
            {
                Initialize(child);
            }
        }

        public void Rescale(double scaleRatio)
        {
            if (child != null)
            {
                // Zoom
                var st = GetScaleTransform(child);
                st.ScaleX = scaleRatio;
                st.ScaleY = scaleRatio;
            }
        }

        public void RunTranslation(double xFrom, double xTo, double yFrom, double yTo, int duration)
        {
            if (child != null)
            {
                DoubleAnimation xAnimation = new DoubleAnimation(xFrom, xTo, TimeSpan.FromMilliseconds(duration));
                DoubleAnimation yAnimation = new DoubleAnimation(yFrom, yTo, TimeSpan.FromMilliseconds(duration));

                // Lance le panorama
                var tt = GetTranslateTransform(child);
                tt.BeginAnimation(TranslateTransform.XProperty, xAnimation);
                tt.BeginAnimation(TranslateTransform.YProperty, yAnimation);
            }
        }

        /// <summary>
        /// Réalise un panorama de l'image passée en paramètre
        /// </summary>
        /// <param name="imageControl"></param>
        /// <param name="imgDrawing"></param>
        /// <param name="duration">Durée du panorama en millisecondes</param>
        public void MakeImagePanorama(System.Drawing.Image imgDrawing, int duration, bool fromLeftToRight = true)
        {
            // Ratios
            double imageRatio = (float)imgDrawing.Width / (float)imgDrawing.Height;

            // Calcul les dimensions de l'image si sa hauteur est celle de l'écran
            double imageHeightResizedFromHeight = (float)SystemParameters.WorkArea.Height;
            double imageWidthResizedFromHeight = (float)SystemParameters.WorkArea.Height * imageRatio;

            // Calcul les dimensions de l'image si sa largeur est celle de l'écran
            double imageHeightResizedFromWidth = (float)SystemParameters.WorkArea.Width / imageRatio;
            double imageWidthResizedFromWidth = (float)SystemParameters.WorkArea.Width;

            // Redimensionne l'image pour que sa hauteur soit celle de l'écran
            Rescale((float)SystemParameters.WorkArea.Height / imageHeightResizedFromWidth);

            // Différence entre la hauteur de l'image et celle de l'écran après redimensionnement à partir de la largeur
            double heightDifferenceResizedFromWidth = (float)SystemParameters.WorkArea.Height - imageHeightResizedFromWidth;
            // Différence entre la largeur de l'image redimensionnée et celle de l'écran
            double widthDifferenceResizedFromHeight = (float)imageWidthResizedFromHeight - (float)SystemParameters.WorkArea.Width;

            if (fromLeftToRight)
            {
                RunTranslation(
                    0,
                    -widthDifferenceResizedFromHeight,
                    -heightDifferenceResizedFromWidth / 2,
                    -heightDifferenceResizedFromWidth / 2,
                    duration);
            }
            else
            {
                RunTranslation(
                    -widthDifferenceResizedFromHeight,
                    0,
                    -heightDifferenceResizedFromWidth / 2,
                    -heightDifferenceResizedFromWidth / 2,
                    duration);
            }
        }

        /// <summary>
        /// Applique une rotation à l'image
        /// </summary>
        /// <param name="angle"></param>
        public void RotateImage(double angle)
        {
            if (child != null)
            {
                var rt = GetRotateTransform(child);
                Point oldRenderTransformOrigin = child.RenderTransformOrigin;
                child.RenderTransformOrigin = new Point(0.5, 0.5);
                rt.Angle = angle;
            }
        }

        /// <summary>
        /// Applique une rotation automatique à l'image en fonction de ses métadonnées.
        /// Redimensionne l'image pour qu'elle passe dans l'écran.
        /// </summary>
        /// <param name="imgDrawing"></param>
        public void AutoRotateImage(System.Drawing.Image imgDrawing)
        {
            double angle = GetRotation(imgDrawing);

            if(angle != 0)
            {
                RotateImage(angle);

                if(angle == 90 || angle == 270)
                {
                    // Ratios
                    double imageRatio = (float)imgDrawing.Width / (float)imgDrawing.Height;

                    // Calcul les dimensions de l'image si sa hauteur est celle de l'écran
                    double imageWidthResizedFromHeight = (float)SystemParameters.WorkArea.Height * imageRatio;

                    // Redimensionne l'image pour que sa hauteur soit celle de l'écran
                    Rescale((float)SystemParameters.WorkArea.Height / imageWidthResizedFromHeight);
                }
            }
            
        }



        #region Child Events

        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .1 : -.1;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                Point relative = e.GetPosition(child);
                double abosuluteX;
                double abosuluteY;

                abosuluteX = relative.X * st.ScaleX + tt.X;
                abosuluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;
            }
        }

        //private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (child != null)
        //    {
        //        var tt = GetTranslateTransform(child);
        //        start = e.GetPosition(this);
        //        origin = new Point(tt.X, tt.Y);
        //        this.Cursor = Cursors.Hand;
        //        child.CaptureMouse();
        //    }
        //}

        //private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (child != null)
        //    {
        //        child.ReleaseMouseCapture();
        //        this.Cursor = Cursors.Arrow;
        //    }
        //}

        void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Reset();
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null)
            {
                if (child.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(child);
                    Vector v = start - e.GetPosition(this);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;
                }
            }
        }

        /// <summary>
        /// Récupère la rotation nécessaire dans les métadonnées de l'image
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private double GetRotation(System.Drawing.Image img)
        {
            double rot = 0;

            if (!img.PropertyIdList.Contains(exifOrientationID))
                return rot;

            var prop = img.GetPropertyItem(exifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);

            if (val == 3 || val == 4)
                rot = 180;
            else if (val == 5 || val == 6)
                rot = 90;
            else if (val == 7 || val == 8)
                rot = 270;

            return rot;
        }

        #endregion
    }
}
