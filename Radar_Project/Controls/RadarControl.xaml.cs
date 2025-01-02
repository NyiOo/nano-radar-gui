using NsrRadarSdk;
using NsrRadarSdk.NsrTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Input;

namespace Radar_Project.Controls
{
    /// <summary>
    /// Interaction logic for RadarControl.xaml
    /// </summary>
    public partial class RadarControl : UserControl , INotifyPropertyChanged
    {
        private readonly Storyboard storyboard;
        //private readonly List<RadarItem> radarItemList;
        private NsrRadar nsrRadar;
        private Int32 _range;
        private Double _cal;
       
        private Pen _pen;
        private Brush _brush;
        private Double _deltaAngle = 45;
        private Point _center;
        private DrawingGroup _dg;
        private ObservableCollection<ITarget> _radarTargets;
        private List<int> _trackTargetID;
        private Dictionary<int, List<Point>> _trackDic;
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _online;       
        //private Pen pen;
        //private List<Point> tr_center_ls; 
        //private List<int> id_ls;
       

        public Int32 Range
        {
            get => _range;
            set
            {
                if (value <= 500) _range = value;
                else throw new ArgumentOutOfRangeException();
            }
        }
        public Double Cal { get => _cal; 
            set
            {
                if (_cal != value)
                {
                    _cal = value;
                    NotifyPropertyChanged("Cal");
                }              
              
            }
        }
        public bool Online { get => _online; 
            set
            {
                _online = value;
                NotifyPropertyChanged("Online");
            } }
        public ObservableCollection<ITarget> Targets
        {
            get => _radarTargets;
            set
            {
                if (_radarTargets != value)
                {
                    _radarTargets = value;
                    NotifyPropertyChanged("Targets");
                }
                   

            }
        }       
        
        public  Brush RadarBrush { get => _brush; set { _brush = value; _pen = new Pen(_brush, 1); } }
       
        public event EventHandler RadarTargetUpdated;

       

        //=================================================== Constructor ===================================================

        public RadarControl()
        {
            InitializeComponent();

            
            storyboard = new Storyboard();   
            //pen = new Pen(Brushes.Red, 1);
            //tr_center_ls = new List<Point>();
            //id_ls = new List<int>();
           
            _dg = new DrawingGroup();
            _radarTargets = new ObservableCollection<ITarget>();
            _trackTargetID = new List<int>();
            _trackDic = new Dictionary<int, List<Point>>();
            RadarBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#42ff00")));

            DataContext = this;            

            CreateAnimation();              //Creating Animation Object            

        }


        //=========================================================== Public Methods ================================================================
        /// <summary>
        /// Assing degree of scanning line to start and stop 
        /// </summary>
        public void SectorScan( )
        {
            storyboard.Stop(this);

            var da = (DoubleAnimation)storyboard.Children[0];
            da.From = _cal-45;
            da.To = _cal+45;            
            da.Duration = new TimeSpan(0, 0, 2);

            storyboard.Begin(this, true);

        }

        /// <summary>
        /// this method will be used with servo motor
        /// </summary>
        public void RotateByAngle()
        {
            var degrees = Enumerable.Range(1, 300).ToList();
            var interval = 100;
            _ = degrees.ForEachWithDelay(i => Task.Run(() => { Cal = Convert.ToDouble(i); Debug.WriteLine(Cal); }), interval);
        }
        /// <summary>
        /// Start scanning line
        /// </summary>
        public void StartAnimation()
        {
            storyboard.Begin(this,true);
        }
        /// <summary>
        /// Stop scanning line
        /// </summary>
        public void StopAnimation()
        {          
            storyboard.Stop(this);
            
        }     
        /// <summary>
        /// Adding target into radarTargetList
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(ITarget item)
        {
            Boolean bFlag = true;

            for (int i = 0; i < _radarTargets.Count; i++)
            {
                if (_radarTargets[i].ID == item.ID)
                {
                    _radarTargets[i] = item;
                    bFlag = false;

                    RadarTargetUpdated?.Invoke(this, null);

                }

            }
            if (bFlag)
                _radarTargets.Add(item);

            DrawTargets();
            
        }      
        /// <summary>
        /// Creaet instance and connect to radar
        /// </summary>
        public void CreateRadarInstance()
        {
            NsrSdk.Instance.CleanUp();

            NsrSdk.Instance.Init(9000, false);
            NsrSdk.Instance.Timeout = 3000;
            NsrSdk.Instance.TargetDetect += Radar_TargetDetect;
            NsrSdk.Instance.RadarOnlineStateChanged += Instance_RadarOnlineStateChanged;

            try
            {
                NsrSdk.Instance.StartReceiveBroadcast(RadarBroadcast);
                //nsrRadar = NsrSdk.Instance.CreateRadar("192.168.10.122", 8100);                
               

            }
            catch (RadarException ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

       
        /// <summary>
        /// Disconnecting to radar
        /// </summary>
        public void Disconnect()
        {
            if (nsrRadar.Online)
                nsrRadar.DisConnect();

            nsrRadar = null;
            NsrSdk.Instance.CleanUp();

            //Online = false;

        }

        /// <summary>
        /// Clear PPI scope
        /// </summary>
        public void ClearPPITargets()
        {
            using (var dc = _dg.Open())
            {
                //do nothing...
            }
        }

       
        //=========================================================== Private Methods ================================================================
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

       
        /// <summary>
        /// Create scanning line in radar
        /// </summary>
        private void CreateAnimation()
        {
            var scan_animation = new DoubleAnimation();
            scan_animation.From = -45.0; //0.0
            scan_animation.To =  45.0;   //359.99
            scan_animation.Duration = new TimeSpan(0,0,2);
            scan_animation.RepeatBehavior = RepeatBehavior.Forever;
            scan_animation.AutoReverse = true;

            

            var scan_keyFrame = new DoubleAnimationUsingKeyFrames()
            {
                Duration = new Duration(new TimeSpan(0, 0, 2)),
                RepeatBehavior = RepeatBehavior.Forever,
               
            };

            scan_keyFrame.KeyFrames.Add(new LinearDoubleKeyFrame() 
            {
                KeyTime = KeyTime.FromPercent(0),
                Value =0.1
            });
            scan_keyFrame.KeyFrames.Add(new LinearDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromPercent(0.5), 
                Value = 1
            });
            scan_keyFrame.KeyFrames.Add(new LinearDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromPercent(1.0),
                Value = 0.1
            });

           
            storyboard.Children.Add(scan_animation);
            storyboard.Children.Add(scan_keyFrame);

            Storyboard.SetTargetName(scan_keyFrame, this.SweepLine.Name);
            Storyboard.SetTargetName(scan_animation, this.SweepLine.Name);

            Storyboard.SetTargetProperty(scan_keyFrame, new PropertyPath(Path.OpacityProperty));
            Storyboard.SetTargetProperty(scan_animation, new PropertyPath("(Path.RenderTransform).(RotateTransform.Angle)"));    
            
        }
        /// <summary>
        /// Get x,y points of circle according to input angle
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private Point GetPoint(Double angle, FormattedText text)
        {
            // Point center = new Point(RenderSize.Width / 2, RenderSize.Height / 2);
            Double radius = RenderSize.Width / 2;
            Double radAngle = angle * Math.PI / 180;

            if (text != null)
            {
                radius += Math.Max(text.Width / 2, text.Height / 2);
            }

            Double x = _center.X + radius * Math.Sin(radAngle);
            Double y = _center.Y + radius * -Math.Cos(radAngle);

            return new Point(x, y);

        }
        /// <summary>
        /// Control radar status accodring by online or not
        /// </summary>
        /// <param name="radar"></param>
        /// <param name="online"></param>
        private void Instance_RadarOnlineStateChanged(NsrRadar radar, bool online)
        {
            if (online)
            {              
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    StartAnimation();
                    Online = true;
                }
                ));   
            }
           
        }
        /// <summary>
        /// cloning and adding target into radarTargetList
        /// </summary>
        /// <param name="radar">radar </param>
        /// <param name="targetList">targets</param>
        private void Radar_TargetDetect(NsrRadar radar, RVS_Target_List targetList)
        {
            DateTime now = DateTime.Now;
            foreach(var item in targetList.Targets)
            {
                int id = (int)item.Id;

                Target target = new Target(id);
                target.Azimuth = Math.Round(item.AzimuthAngle,2);

                // north offset 
                if (_cal < 0)
                {
                    target.Azimuth = target.Azimuth + (int)(360.0 + _cal);
                }
                else
                    target.Azimuth += (int)_cal;

                target.Distance = Math.Round( item.Distance,2);

                target.Speed = Math.Round( item.SpeedY,2);               
               
                if(item.SpeedY < 0 )
                    target.Approach = true;  
                else
                    target.Approach = false;

                var tempTarget = target.Clone() as Target;                

               this.Dispatcher.BeginInvoke(new Action(() => AddItem(tempTarget)));

                Debug.WriteLine("Target is {0}, {1}, {2}",tempTarget.ID, tempTarget.Distance, tempTarget.Azimuth);
               

            }
        }
        /// <summary>
        /// if radar'state is online , repaint UI according by radar's heart beat
        /// </summary>
        /// <param name="radar">radar</param>
        /// <param name="info">radar info</param>
        private void RadarBroadcast(NsrRadar radar, ref RVS_PARAM_BROADCAST info)
        {
            if(radar.Online == true)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>                        //clear target list and PPI Scope
                {
                    _radarTargets.Clear();
                    DrawTargets();
                }));
            }
            else
            {
                nsrRadar = radar;
                nsrRadar.Connect();
               
              
            }

           

        }
        /// <summary>
        /// convert into x,y according by azimuth and distance
        /// </summary>
        /// <param name="azimuth"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private Point AzE12XY(Double azimuth, Double distance)
        {
            var Hand = (RenderSize.Height / 2);
            var dd = Hand / _range;
            var dis = (Double)(dd * distance);
            Double x, y;

            if (azimuth >= 0 && azimuth <= 180)
            {
                x = _center.X + (Double)(dis * Math.Sin(Math.PI * azimuth / 180));
                y = _center.Y - (Double)(dis * Math.Cos(Math.PI * azimuth / 180));
            }
            else
            {
                x = _center.X - (Double)(dis * -Math.Sin(Math.PI * azimuth / 180));
                y = _center.Y - (Double)(dis * Math.Cos(Math.PI * azimuth / 180));
            }

            return new Point(x, y);
        }
        /// <summary>
        /// Draw target and track
        /// </summary>
        private void DrawTargets()
        {
            List<Point> trackPts;

            using (var dc = _dg.Open())
            {
                foreach (var item in _radarTargets)
                {
                    //==================== for target =================================
                    Point cp = AzE12XY(item.Azimuth, item.Distance);
                    Point center = new Point((cp.X), (cp.Y));
                    Brush brush = null;

                    if (!item.Approach)
                        brush = Brushes.Green;
                    else
                        brush = Brushes.Red;

                    dc.DrawEllipse(brush, new Pen(Brushes.Yellow, 1), new Point(center.X, center.Y), 5, 5);

                    var flag = _trackDic.TryGetValue(item.ID, out trackPts);

                    if (flag)   //trackFlag
                    {

                        trackPts.Add(center);

                        Debug.WriteLine("{0} has {1}", item.ID, trackPts.Count);

                        if (trackPts.Count > 0)
                        {
                           for(int i=0; i< trackPts.Count; i+=5)
                           {
                                
                              dc.DrawEllipse(brush, new Pen(Brushes.Yellow, 1), new Point(trackPts[i].X, trackPts[i].Y), 5, 5);
                                  
                           }
                               
                        }

                        if(trackPts.Count > 50)
                        {
                            trackPts.RemoveAt(0);
                        }
                        
                    }
                   


                }

            }
        }   

      

        /// <summary>
        /// Compare target's (x,y) and mouse click position (x,y)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="pt"></param>
        /// <returns>boolean</returns>
        private Boolean HitTestVisual(Point target, Point pt)
        {
            var X = Math.Round(target.X, 0);
            var Y = Math.Round(target.Y, 0);
            var offset = 30;
           
            //Debug.WriteLine("t.X={0},t.Y={1}", X, Y);

            if(pt.X > X-offset && pt.X < X + offset)
            {
                if(pt.Y > Y-offset && pt.Y < Y + offset)
                {
                    //var str = string.Format("t {0},{1} and m {2},{3}", X, Y, pt.X, pt.Y);
                    //Debug.WriteLine(str);

                    return true;
                }
            }
            return false;
        }
       
      
       
        //========================================================= Override Methods =====================================================================
        /// <summary>
        /// Rendering Radar Ring, Angle Text and Sectors
        /// </summary>
        /// <param name="dc"></param>
        protected override void OnRender(DrawingContext dc)
        {            
            dc.DrawDrawing(_dg);
            
            int step = (int)(360 / _deltaAngle);

            _center = new Point(RenderSize.Width / 2, RenderSize.Height / 2);

            dc.DrawEllipse(null, _pen, _center, RenderSize.Width / 2, RenderSize.Height / 2);

            for (int i = 0; i < step; i++)
            {
                var angle = i * _deltaAngle;
                var p1 = GetPoint(angle, null);

                //Lines and Circles
                dc.DrawLine(_pen, _center, p1);
                Double radius = RenderSize.Width * 0.5 * i / step;
                dc.DrawEllipse(null, _pen, _center, radius, radius);

                //Angle Text
                FormattedText text = new FormattedText("" + (i * _deltaAngle), CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight, new Typeface("Pyidaungsu"), 12, Brushes.Yellow);

                Point p2 = GetPoint(angle, text);
                p2.X -= text.Width / 2;
                p2.Y -= text.Height / 2;
                dc.DrawText(text, p2);

                //Distance Text
                String distance = (_range / step * (step - i)).ToString();
                var x = _center.X + 5;
                var y = _center.Y - (RenderSize.Width / 2 / step * (step - i));
                FormattedText dis_txt = new FormattedText(distance + "m", CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight, new Typeface("Pyidaungsu"), 12, Brushes.Yellow);

                dc.DrawText(dis_txt, new Point(x, y));
            }
            
        }

       

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(this);

            var item = _radarTargets.AsEnumerable<ITarget>().Where(i =>
            {
                var target = AzE12XY(i.Azimuth, i.Distance);

                return HitTestVisual(target, pt);

            }).FirstOrDefault();

            if(item!=null)
            {
                if (_trackDic.ContainsKey(item.ID))
                    _trackDic.Remove(item.ID);
                else
                    _trackDic.Add(item.ID, new List<Point>());

            }

        }

    }

    
    
}
