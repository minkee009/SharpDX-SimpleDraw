namespace SharpDXSimple
{
    public partial class Form1 : Form
    { 
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs args)
        {
            Invalidate();
        }
    }
}