   M S B A M L   `   `   `  ����J  FSimpleAntivirus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:SimpleAntivirus.GUI.Views.PagesSimpleAntivirus.GUI.Views.Pages  5      I)clr-namespace:SimpleAntivirus.GUI.HelpersSimpleAntivirus.GUI.Helpers  5      :    4SimpleAntivirus.GUI.Views.Pages.QuarantinedItemsPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:SimpleAntivirus.GUI.Views.Pages     5      =helpers)clr-namespace:SimpleAntivirus.GUI.Helpers     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5	      A =Wpf.Ui, Version=3.0.4.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  5
      -   5          N� Title$  QuarantinedItemsPage��5         ApplicationBackgroundBrush  Wpf.Ui.Markup.Design   
Background# �   5        TextFillColorPrimaryBrush   
Foreground# �  5       N� 
Foreground# �  5      $	��487��5       � 	Resources 5        Wpf.Ui.Controls.Button �� BasedOn �� 
TargetType �� Property �� Value*  $System.Windows.Controls.DataGridCell  BoolToVisibilityConverter9   3SimpleAntivirus.GUI.Helpers.InverseBooleanConverter  InverseBooleanConverterE   ?SimpleAntivirus.GUI.Helpers.InverseBooleanToVisibilityConverter ' #InverseBooleanToVisibilityConverter��%w  '        7 '  �     &	     &	     &	 .    �� 5   
   8   "  6>   .�5      ��  � �� �	#FF2C05F26+   �� 5       � �� �	#E4FFFFFF6+   �� 5       �@ MouseOverBackground �	#80476EE964   �� 5       � ��$
 Bold"�6+   5      �� 5   
   "  6   5      �� 5   
   6'     5    
   6*     5!   
   66   5"      .��5$      �� .+�5%   
     Wpf.Ui.Controls.TextBlock  $
��Wrap=�6   $��20(�6+   $]��WView the currently quarantined items and either unquarantine, whitelist or delete them.��69   $��0,0,0,20q�6�   &   System.Windows.Controls.DataGrid  5&   
   -   5        	  Name$	 QuarantinedItemsDataGrid��5&      $	��287��65   
   AutoGenerateColumns$
 False��6B      
IsReadOnly$
 True��6^      Columns 5'      0  *System.Windows.Controls.DataGridTextColumn  5(      ,	  &System.Windows.Controls.DataGridColumn 	  Header$ Original File Path��6%    	  Width5
  /System.Windows.Controls.DataGridLengthConverter$ *
 6A   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6K   �� *OriginalFilePath+  5)      $ Date Quarantined��6%   $ 0.25*
 6?    6M   �� *QuarantineDate+5+        5,   
   -   5          Name$ SelectLabel��5,   +   $
��Wrap=�6   $��No Items Selected��6@   $��14(�6Y   $��Italic$�6g   � 5-   
   $��44��6    � ColumnDefinitions 5.      �� 5/      $��* �6#   �� 50      $��* �6#   �� 51      $��* �6#   .+�53        -   5        $��Unquarantine��53      $��0,10,0,0q�6/   $	��Top=�6A   $��Right=�6Y   ���	#FF2204B8y�6�   �� *IsBusy+ �� 	Converter# [   54      -   5        $��1��54      $��Add to Whitelist��6(   $��1,10,0,0q�6C   $	��Top=�6U   $��Center=�6m   ���	#FF2A5824y�6�   �� *IsBusy+# [   55      -   5        $��2��55      $��Delete��6(   $��0,10,0,0q�69   $	��Top=�6K   $
��Left=�6c   ���#ba1111   MouseOverBackground
 �����y�6�   �� *IsBusy+# [ 56      � 57   
   $��68��6    58      �� 59      $
��0.3* �6#   �� 5:      $��* �6#   .+�5<      "  Wpf.Ui.Controls.ProgressRing  $��0��6    J� RenderTransformOrigin$ 0.5,0.5&�6.   $��Center=�6N   ���	#FF2204B8$	��Top=�6�   y�6�   �� *IsBusy+# [ z�5=      V� 5>      .~�5?      �� �� 5@      �� 5A      -   5        
rotateRing�� �� Angle$ 0Q�5A   >   T� 5B      5C       � Triggers 5F      :� 5G       :� RoutedEvent$ ui:ProgressRing.Loaded��6#   .[�5H      �� .��5I      �� .��5J   "   W�  �� 
TargetName$ 
rotateRing��5K   !    �� TargetProperty$ Angle�5L   !    W� From 05M   !   	 W� To 3605N   !    g� Duration$ 0:0:1D�5O   !    g� RepeatBehavior$ Forever��5P   !   5Q      5R      5S      5U        5V      $
��Wrap=�6   $��20(�6/   $��1��6=   $��
0,3,157,12q�6M   y�6a   �� *IsBusy+# [ �� 5W      $/�)Please wait for the process to complete. ��6   �� 5X      �� 5Y       �� 5Z      $�This may take some time.��6   5[      5\      5]      5^      