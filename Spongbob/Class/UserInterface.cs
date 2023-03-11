namespace Spongbob
{
    public partial class UserInterface : Form
    {
        public UserInterface()
        {
            this.DoubleBuffered = true;
            this.Resize += new EventHandler(UserInterface_Resize);

            InitializeComponent();

           
            initBackground();
            InitTable(5, 5);
        }

        protected override void OnPaint(PaintEventArgs e) { }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return handleParam;
            }
        }

    
    }
}