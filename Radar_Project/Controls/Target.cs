using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Radar_Project.Controls
{
    public class Target : ITarget , ICloneable
    {
        int _id;
        double _distance;
        double _azimuth, _speed;
        double _elevation;
        bool _depart;
        bool _approach;
        DateTime _created = DateTime.Now;       
        private Pen pen = new Pen(Brushes.Red, 1);
        private List<Point> _trackPoints;

        public int ID => _id;
        public double Azimuth { 
            get => _azimuth;
            set
            {
                _azimuth = value;

                if (_azimuth < 0)                
                    _azimuth += 360;                
                else if (_azimuth >= 360)
                    _azimuth %= 360;
            }
        }
        public double Elevation { get => _elevation; set => _elevation = value; }

        public double Distance { get => _distance; set => _distance = value; }

        public bool Approach { get => _approach; set => _approach = value; }

        public bool Depart { get => _depart; set => _depart = value; }

        public DateTime Created { get => _created; set => _created = value; }

        public double Speed { get => _speed; set => _speed = value; }
        public List<Point> TrackPoints => _trackPoints;

        public Target(int id)
        {
            _id = id;

            _trackPoints = new List<Point>();
        }

        

        public int CompareTo(ITarget other)
        {
            if (this.ID == other.ID)
                return 1;
            else
                return 0;
        }

        public object Clone()
        {
            var target = (Target)this.MemberwiseClone();
            target.TrackPoints.AddRange(_trackPoints);
            return target;
        }
    }
}
