   M S B A M L   `   `   `  ����J  FSimpleAntivirus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:SimpleAntivirus.GUI.Views.PagesSimpleAntivirus.GUI.Views.Pages  5      ;    5SimpleAntivirus.GUI.Views.Pages.ProtectionHistoryPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:SimpleAntivirus.GUI.Views.Pages     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      A =Wpf.Ui, Version=3.0.4.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  5      -   5          N� Title$  ProtectionHistoryPage��5	         ApplicationBackgroundBrush  Wpf.Ui.Markup.Design   
Background# �   5        TextFillColorPrimaryBrush   
Foreground# �  5       N� 
Foreground# �  5       � 	Resources 5      *  $System.Windows.Controls.DataGridCell �� 
TargetType��%+   '        �� 5   
   "  6   5      5      .��5      �� .+�5   
     Wpf.Ui.Controls.TextBlock  $��	0,10,0,50q�6   $
��Wrap=�6*   $	��Top=�6>   $��20(�6V   $K��EView information about past security events related to your computer.��6d   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ ProtectionHistoryDataGrid��5      $	��234��66      AutoGenerateColumns$ False��6C      
IsReadOnly$
 True��6_   	   Columns	 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn
   Header$
 	Component��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$
 0.2* 6V   1  +System.Windows.Controls.DataGridBoundColumn   Binding 68   �� *	Component+6V     5      $
 Severity��6%   $
 0.2* 6T    67   �� *
Severity+6T     5      $
 Message��6%   $
 0.7* 6[    66   �� *TruncatedMessage+6[     5      $
 	TimeStamp��6%   $
 0.3* 6V    68   �� *	TimeStamp+6V   5!      	  Wpf.Ui.Controls.Button	  5"   
   -   5         	 Name$ DetailsButton��5"      $y�Hidden=�6+   $��Details��6?   $��Center=�6Q   ���	#FF2C05F2���	#E4FFFFFF$��0,50,0,0q�6�   $	��188��6�   6�   	  5$   
   -   5        $y�Hidden=�5$      $��Send Test Alert��6(   ���	#FF2C05F2���	#E4FFFFFF$
��Bold"�6�    	  MouseOverBackground �	#80476EE9$��0,50,0,0q�6�   $	��Top=�6�   $��Center=�6�   5%      5&      