using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AI.SteeringBehaviors.Core;

namespace AI.SteeringBehaviors.ApplicationPresentation
{
    public partial class Main : Form
    {
        #region Non designer fields

        int frameCount;
        int shipCount;
        float deltaTimeAccumulator;
        float oneSecondAccumulator;
        const float UpdateTimeStep = 0.04f;
        const int sliderPrecision = 100;
        Random rng;
        List<ITaskForce> taskForces;

        #region Rendering stuff

        Graphics renderControlGraphics;
        Graphics backbufferGraphics;
        Bitmap backbuffer;
        Bitmap backgroundTexture;
        Bitmap shipTexture;

        #endregion

        #endregion

        public Main()
        {
            InitializeComponent();
            InitializeResources();
            deltaTimeAccumulator = 0;
            taskForces = new List<ITaskForce>(5);
            rng = new Random();
            // nothing selected, disable controls
            numShipsSelector.Enabled = false;
            flockRadiusSelector.Enabled = false;
            colorButton.Enabled = false;
            alignmentWeightSelector.Enabled = false;
            cohesionWeightSelector.Enabled = false;
            separationWeightSelector.Enabled = false;
        }

        private void InitializeResources()
        {
            backgroundTexture = new Bitmap("ApplicationPresentation/spacefield.png");
            shipTexture = new Bitmap("ApplicationPresentation/ship.png");
            backbuffer = new Bitmap(renderControl.Width, renderControl.Height, PixelFormat.Format32bppArgb);
            backbufferGraphics = Graphics.FromImage(backbuffer);
            renderControlGraphics = renderControl.CreateGraphics();
        }

        public void Render()
        {
            lock (this)
            {
                backbufferGraphics.ResetTransform();
                backbufferGraphics.DrawImageUnscaled(backgroundTexture, Point.Empty);

                foreach (ITaskForce tf in taskForces)
                {
                    float[][] matrix = {   new float[] {tf.Color.R / 255f, 0, 0, 0, 0},
                                               new float[] {0, tf.Color.G / 255f, 0, 0, 0},
                                               new float[] {0, 0, tf.Color.B / 255f, 0, 0},
                                               new float[] {0, 0, 0, 1, 0},
                                               new float[] {0, 0, 0, 0, 1}};
                    ColorMatrix modulation = new ColorMatrix(matrix);
                    ImageAttributes imgAtt = new ImageAttributes();
                    imgAtt.SetColorMatrix(modulation);
                    foreach (MovingObject ship in tf.Boids)
                    {
                        // set up matricies to determine where to render
                        backbufferGraphics.ResetTransform();

                        //graphics.TranslateTransform(shipTexture.Width >> 1, shipTexture.Height >> 1, System.Drawing.Drawing2D.MatrixOrder.Append);

                        // setup rotation matrix and
                        backbufferGraphics.RotateTransform((ship.Heading) * 180f / Convert.ToSingle(Math.PI), System.Drawing.Drawing2D.MatrixOrder.Append);

                        // setup translate matrix and
                        // apply translation to transform
                        backbufferGraphics.TranslateTransform(ship.Position.X, ship.Position.Y, System.Drawing.Drawing2D.MatrixOrder.Append);
                        //graphics.DrawImageUnscaledAndClipped(shipTexture, new Rectangle(new Point(20,20), shipTexture.Size));

                        backbufferGraphics.DrawImage(shipTexture, new Rectangle(-shipTexture.Width >> 1, -shipTexture.Height >> 1, shipTexture.Width, shipTexture.Height),
                            0, 0, shipTexture.Width, shipTexture.Height, GraphicsUnit.Pixel, imgAtt);
                    }
                }

                //present the backbuffer
                renderControlGraphics.DrawImageUnscaled(backbuffer, Point.Empty);

            }

            // increment the frame counter for FPS display
            ++frameCount;
        }

        private void UpdateTaskForces()
        {
            // Let's not try to update if Main hasn't finished initializing
            if (null == taskForces)
                return;

            shipCount = 0;

            // update all objects
            foreach (ITaskForce tf in taskForces)
            {
                lock (this)
                {
                    shipCount += tf.Boids.Count;

                    // perform steering updates (flocking)
                    tf.Update(UpdateTimeStep);

                    // bounds check
                    foreach (MovingObject ship in tf.Boids)
                    {
                        Vector3 position = ship.Position;
                        if (position.X + ship.CollisionRadius < 0)
                            position.X = renderControl.Width + ship.CollisionRadius;
                        if (position.X - ship.CollisionRadius > renderControl.Width)
                            position.X = -ship.CollisionRadius;
                        if (position.Y + ship.CollisionRadius < 0)
                            position.Y = renderControl.Height + ship.CollisionRadius;
                        if (position.Y - ship.CollisionRadius > renderControl.Height)
                            position.Y = -ship.CollisionRadius;
                        ship.Position = position;
                    }

                    totalShipsInfo.Text = "Total Ships: " + shipCount.ToString();
                }
            }
        }

        private void updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (false == updateTimer.Enabled)
                return;
            float deltaTime = Convert.ToSingle(updateTimer.Interval) / 1000.0f; // Update calls "should" always be 100Hz
            deltaTimeAccumulator += deltaTime;
            oneSecondAccumulator += deltaTime;
            if (deltaTimeAccumulator > UpdateTimeStep)
            {
                // update accumulator and prevent multiple update calls
                deltaTimeAccumulator -= UpdateTimeStep;
                if (deltaTimeAccumulator > UpdateTimeStep)
                    deltaTimeAccumulator = 0;

                UpdateTaskForces();

            }
            if (oneSecondAccumulator >= 1.0f)
            {
                oneSecondAccumulator = 0;
                lock (this)
                {
                    FPSInfo.Text = "FPS: " + frameCount.ToString();
                    frameCount = 0;
                }
            }
        }

        private void addTaskForceButton_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                ITaskForce tf;
                if (exampleImplementationButton.Checked)
                    tf = new ExampleTaskForce();
                else
                    tf = new TaskForce();
                tf.AveragePosition = new Vector3(Convert.ToSingle(
                    rng.NextDouble() * renderControl.Width), Convert.ToSingle(
                    rng.NextDouble() * renderControl.Height), 0);
                tf.SetShipAmount(20);
                taskForces.Add(tf);
                taskForceSelector.Items.Add(tf.ToString());
                taskForceSelector.SelectedIndex = taskForceSelector.Items.Count - 1;
            }
        }

        private void removeTaskForceButton_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                int index = taskForceSelector.SelectedIndex;
                if (index >= 0)
                {
                    taskForceSelector.Items.RemoveAt(index);
                    taskForces[index].RemoveAllShips();
                    taskForces.RemoveAt(index);
                    if (0 != taskForces.Count)
                    {
                        if (index > taskForces.Count - 1)
                            index = taskForces.Count - 1;
                        taskForceSelector.SelectedIndex = index;
                    }
                    else taskForceSelector.SelectedIndex = -1;
                }
            }
        }

        private void numShipsSelector_ValueChanged(object sender, EventArgs e)
        {
            ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
            lock (this)
            {
                tf.SetShipAmount(Convert.ToInt32(numShipsSelector.Value));
                taskForceSelector.Items[taskForceSelector.SelectedIndex] = tf.ToString();
            }
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
            ColorDialog cd = new ColorDialog();
            lock (this)
            {
                renderControl.Enabled = false;
                cd.ShowDialog(this);
                tf.Color = cd.Color;
                renderControl.Enabled = true;
            }
            taskForceSelector.Items[taskForceSelector.SelectedIndex] = tf.ToString();
            colorButton.BackColor = cd.Color;
            colorButton.ForeColor = GetReadableForeColor(colorButton.BackColor);
        }

        private void alignmentWeightSelector_ValueChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
                tf.AlignmentStrength = Convert.ToSingle(alignmentWeightSelector.Value)
                    / sliderPrecision;
                alignmentWeightInfo.Text = tf.AlignmentStrength.ToString("F");
            }
        }

        private void cohesionWeightSelector_ValueChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
                tf.CohesionStrength = Convert.ToSingle(cohesionWeightSelector.Value)
                    / sliderPrecision;
                cohesionWeightInfo.Text = tf.CohesionStrength.ToString("F");
            }
        }

        private void separationWeightSelector_ValueChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
                tf.SeparationStrength = Convert.ToSingle(separationWeightSelector.Value)
                    / sliderPrecision;
                separationWeightInfo.Text = tf.SeparationStrength.ToString("F");
            }
        }

        private void flockRadiusSelector_ValueChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
                tf.FlockRadius = Convert.ToSingle(flockRadiusSelector.Value);
            }
        }

        private void exampleImplementationButton_CheckedChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                if (exampleImplementationButton.Checked)
                {
                    List<ITaskForce> copy = new List<ITaskForce>(taskForces.Count);
                    for (int i = 0; i < taskForces.Count; ++i)
                    {
                        ITaskForce tf = new ExampleTaskForce();
                        tf.CopyFrom(taskForces[i]);
                        copy.Add(tf);
                    }
                    taskForces = copy;
                }
                else
                {
                    List<ITaskForce> copy = new List<ITaskForce>(taskForces.Count);
                    for (int i = 0; i < taskForces.Count; ++i)
                    {
                        ITaskForce tf = new TaskForce();
                        tf.CopyFrom(taskForces[i]);
                        copy.Add(tf);
                    }
                    taskForces = copy;
                }
            }
        }

        private Color GetReadableForeColor(Color backColor)
        {
            if (backColor.GetBrightness() > 0.5f)
                return Color.Black;
            return Color.White;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            updateTimer.Enabled = false;
        }

        private void taskForceSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (-1 == taskForceSelector.SelectedIndex)
            {
                // nothing selected, disable controls
                numShipsSelector.Enabled = false;
                colorButton.Enabled = false;
                alignmentWeightSelector.Enabled = false;
                cohesionWeightSelector.Enabled = false;
                separationWeightSelector.Enabled = false;
                flockRadiusSelector.Enabled = false;
            }
            else
            {
                lock (this)
                {
                    // reset controls to selected taskforce's information
                    ITaskForce tf = taskForces[taskForceSelector.SelectedIndex];
                    numShipsSelector.Enabled = true;
                    numShipsSelector.Value = tf.Boids.Count;
                    colorButton.Enabled = true;
                    colorButton.BackColor = Color.FromArgb(tf.Color.ToArgb());
                    colorButton.ForeColor = GetReadableForeColor(colorButton.BackColor);
                    alignmentWeightSelector.Enabled = true;
                    alignmentWeightSelector.Value = Clamp(Convert.ToInt32(tf.AlignmentStrength * sliderPrecision), alignmentWeightSelector.Minimum, alignmentWeightSelector.Maximum);
                    alignmentWeightInfo.Text = tf.AlignmentStrength.ToString("F");
                    cohesionWeightSelector.Enabled = true;
                    cohesionWeightSelector.Value = Clamp(Convert.ToInt32(tf.CohesionStrength * sliderPrecision), cohesionWeightSelector.Minimum, cohesionWeightSelector.Maximum);
                    cohesionWeightInfo.Text = tf.CohesionStrength.ToString("F");
                    separationWeightSelector.Enabled = true;
                    separationWeightSelector.Value = Clamp(Convert.ToInt32(tf.SeparationStrength * sliderPrecision), separationWeightSelector.Minimum, separationWeightSelector.Maximum);
                    separationWeightInfo.Text = tf.SeparationStrength.ToString("F");
                    flockRadiusSelector.Enabled = true;
                    flockRadiusSelector.Value = Clamp(Convert.ToDecimal(tf.FlockRadius), flockRadiusSelector.Minimum, flockRadiusSelector.Maximum);
                }
            }
        }

        private T Clamp<T>(T value, T min, T max) where T : System.IComparable<T>
        {
            return (value.CompareTo(min) < 0) ? min : (value.CompareTo(max) > 0) ? max : value;
        }

    }
}
