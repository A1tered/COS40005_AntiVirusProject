   M S B A M L   `   `   `  ����J  FSimpleAntivirus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:SimpleAntivirus.GUI.Views.PagesSimpleAntivirus.GUI.Views.Pages  5      5    /SimpleAntivirus.GUI.Views.Pages.AlertReportPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:SimpleAntivirus.GUI.Views.Pages     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      A =Wpf.Ui, Version=3.0.4.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  5      -   5          N� Title$  Alert History��5	         ApplicationBackgroundBrush  Wpf.Ui.Markup.Design   
Background# �   5        TextFillColorPrimaryBrush   
Foreground# �  5       N� 
Foreground# �  5       � 	Resources 5        Wpf.Ui.Controls.Button �� BasedOn �� 
TargetType �� Property �� Value*  $System.Windows.Controls.DataGridCell��%   '        7 '  �     �� 5   
   8   "  6>   .�5      ��  � �� �	#FF2C05F26+   �� 5       � �� �	#E4FFFFFF6+   �� 5       �@ MouseOverBackground �	#80476EE964   �� 5       � ��$
 Bold"�6+   5      �� 5   
   "  6   5      5      .��5      � 	 � ColumnDefinitions	 5   
   �� 5       �� 5!      �� 5"      
 � RowDefinitions
 5$   
   �� 5%      .+�5'   
     Wpf.Ui.Controls.TextBlock  $��	0,10,0,50q�6   $
��Wrap=�6*   $��1��6>   $��Center=�6N   $	��Top=�6k   $��40(�6�   �� 5(       � Language$ en-au�6   $�Threat Report��6#   5)        5*   
   -   5          Name$ DetailsButton��5*      $y�Hidden=�6+   $��Details��6?   $��Center=�6Q   $��0,50,0,0q�6n   $	��188��6�     5+   
   -   5        $
��Back��5+      $	��Top=�6#   $��0��6;   $��	20,20,0,0q�6K   $	��132��6^   6j     Wpf.Ui.Controls.Card  5,   
   $��Stretch=�6   $��	0,108,0,0q�61   $	��Top=�6D   $��0��6\   $��3��6l   $	��706��6�   $	��296��6�   .��5-      � 
 5.      �� 5/      �� 50      �� 51      �� 52      �� 53      	 55      �� 56      $��* �6'   �� 57      $��* �6'   .+�59        -   5          Name$ TimeStampBlock��59      $��11/08/2024 9:21pm��67   $��10(�6P   $��Center=�6^   $��2��6{     5:      -   5        $ 
ThreatType��5:      $��Threat Type:��63   $��20(�6G   $��Center=�6U   $��2��6r     5;      -   5        $ Severity��5;      $��	Severity:��61   $��20(�6B   $��Center=�6P   $��1��6m   $��2��6}     5<      -   5        $ Message��5<      $��Message:��60   $��10(�6@   $
��Left=�6N    �� TextAlignment$
 Left=�6i   $��3��6~   $��2��6�   $
��Wrap=�6�     5=      -   5        $ SuggestedAction��5=      $��SuggestedAction:��68   $��10(�6P   $
��Left=�6^   $
 Left=�6y   $��4��6�   $��2��6�   5?      5@      5A      5B      