   M S B A M L   `   `   `  ����J  FSimpleAntivirus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:SimpleAntivirus.GUI.Views.PagesSimpleAntivirus.GUI.Views.Pages  5      :    4SimpleAntivirus.GUI.Views.Pages.IntegrityResultsPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:SimpleAntivirus.GUI.Views.Pages     5      A =Wpf.Ui, Version=3.0.4.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  5      -   5        $��10q�5        N� Title$  IntegrityResultsPage��5
       � 	Resources 5        Wpf.Ui.Controls.Button �� BasedOn �� 
TargetType �� Property �� Value��%�   '        7 �� 5   
   8   "  6>   .�5      ��  � �� �	#FF2C05F26+   �� 5       � �� �	#E4FFFFFF6+   �� 5       �@ MouseOverBackground �	#80476EE964   �� 5       � ��$
 Bold"�6+   5      5      .��5      �  � RowDefinitions 5   
   �� 5      �� 5       � ColumnDefinitions 5   
   �� 5      �� 5      $	��290��6   �� 5      .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ DataShow��5      $��0��6%   $��Stretch=�65   $	   HorizontalScrollBarVisibility$	 Visible=�6S   $��3��6{   $��34,64,34,64q�6�   $��2��6�   
   AutoGenerateColumns$
 False��6�      
IsReadOnly$
 True��6�   $	��300��6�      Columns 5       0  *System.Windows.Controls.DataGridTextColumn  5!      ,  &System.Windows.Controls.DataGridColumn   Header$ 	Directory��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6]   1  +System.Windows.Controls.DataGridBoundColumn   Binding 68   �� *DisplayDirectory+6]     5"      $ Size Change��6%   $
 0.3* 6Y    6:   �� *
SizeChange+6Y     5#      $ Missing��6%   $
 0.3* 6R    66   �� *	Missing+6R     5$      $ SigTime��6%   $
 0.7* 6R    66   �� *	SigTime+6R   5&      "  Wpf.Ui.Controls.ProgressRing  5'   
   -   5          Name$ ProgressAdd��5'      $y�Hidden=�6/    J� RenderTransformOrigin$ 0.5,0.5&�6C   $��1��6d   $
��Left=�6t   $��	154,9,0,0q�6�   $	��Top=�6�   $��24��6�   $��24��6�   z�5(      �� 5)      -   5        Rotator�� �� Angle$ 0Q�5)   3   5+        5,   
   -   5        $��0��5,      $��Center=�6$   $
��Back��6A   $��	34,21,0,0q�6P   $	��Top=�6c   $	��221��6{   6�     Wpf.Ui.Controls.TextBlock  5-   
   $��1��6   $��Center=�6'   $��0,10,0,0q�6D   $
��Wrap=�6V   $��Integrity Results��6j   $	��Top=�6�   $��33(�6�   $	��234��6�   ���	#FFE6DCDC6�   5/      50      