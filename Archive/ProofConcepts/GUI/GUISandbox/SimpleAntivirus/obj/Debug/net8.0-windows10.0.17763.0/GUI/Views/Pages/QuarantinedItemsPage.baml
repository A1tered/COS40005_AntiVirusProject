   M S B A M L   `   `   `  ����J  FSimpleAntivirus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:SimpleAntivirus.GUI.Views.PagesSimpleAntivirus.GUI.Views.Pages  5      :    4SimpleAntivirus.GUI.Views.Pages.QuarantinedItemsPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:SimpleAntivirus.GUI.Views.Pages     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      A =Wpf.Ui, Version=3.0.4.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  5	      -   5          N� Title$  QuarantinedItemsPage��5
         ApplicationBackgroundBrush  Wpf.Ui.Markup.Design   
Background# �   5        TextFillColorPrimaryBrush   
Foreground# �  5       N� 
Foreground# �  5      .��5      �� .+�5   
     Wpf.Ui.Controls.TextBlock  $
��Wrap=�6   $��20(�6+   $]��WView the currently quarantined items and either unquarantine, whitelist or delete them.��69   $��0,0,0,20q�6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ QuarantinedItemsDataGrid��5      $	��319��65      AutoGenerateColumns$ False��6B      
IsReadOnly$
 True��6^      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Original File Path��6%   	   Width5  /System.Windows.Controls.DataGridLengthConverter$	 * 6A   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 6K   �� *OriginalFilePath+  5      $ Date Quarantined��6%   $
	 0.2* 6?   
 6L   �� *QuarantineDate+5      5      5      