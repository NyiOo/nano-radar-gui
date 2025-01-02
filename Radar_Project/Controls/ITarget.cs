using System;
using System.Collections.Generic;
using System.Windows;

namespace Radar_Project.Controls
{
    public interface ITarget : IComparable<ITarget>
    {
        int ID { get; }
        double Azimuth { get; set; }
        double Elevation { get; set; }
        double Distance { get; set; }
        bool Approach { get; set; }
        bool Depart { get; set; }
        DateTime Created { get; }      
        List<Point> TrackPoints { get; }



    }
}
