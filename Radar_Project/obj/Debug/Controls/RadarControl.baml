   M S B A M L   `   `   `  ����H  DRadar_Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:Radar_Project.ControlsRadar_Project.Controls  5      )    #Radar_Project.Controls.RadarControl   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6local$clr-namespace:Radar_Project.Controls     5      $	��500��5      $	��500��6   $	��700��6-   $	��700��6=   .��5      � -   5          �Name$
  grid��5       � 	Resources 5
   
      SweepingLineBrush 
� GradientOrigin 
� RadiusX 
� RadiusY � Offset � Color��%�   &	        
� 5      $	 0,1&�6<   $ 1Q�6Q   $ 1Q�6]   .��5      � $ 0Q�6   $ #42ff00��6*   � 5      $ 1Q�6   $ Transparent��6*   5      5       � RowDefinitions 5   
   �� 5      �� 5       � ColumnDefinitions 5   
   �� 5      �� 5      .+�5   
   �� $��50��6   $��/Image/radar.png��6   $��0��65   $��0��6B   $��2��6R   $��2��6c   	 J� Opacity$
	 0.75Q�6w   F� 5   
   -   5        
 F�Name$
 	SweepLine��5      #��[  6"   $��0��6L    �� Stretch$ Uniform=�6`   $
��Left=�6r   $��0��6�   $��1��6�    J� RenderTransformOrigin$	 0,1&�6�   =����        �    ��- ���?|�?    ���?|�?    �� 	z�5      �� 5       �� Angle 6"   �� *Angle+5      �� 5    
   $��0��6   $��0��6&   $��2��66   $��2��6G   5!      5"      