   M S B A M L   `   `   `  ����J  FSimpleAntivirus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:SimpleAntivirus.GUI.Views.PagesSimpleAntivirus.GUI.Views.Pages  5      3    -SimpleAntivirus.GUI.Views.Pages.BlacklistPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ?local-clr-namespace:SimpleAntivirus.GUI.Views.Pages     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      A =Wpf.Ui, Version=3.0.4.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  5        N� Title$  BlacklistPage��5	         ApplicationBackgroundBrush  Wpf.Ui.Markup.Design   
Background# �   5        TextFillColorPrimaryBrush   
Foreground# �  5       N� 
Foreground# �  5      .��5      �� .+�5   
     Wpf.Ui.Controls.TextBlock  $
��Wrap=�6   $��20(�6+   �� 5       � Language$ en-au�6   $���Notice something malicious that Simple Antivirus was unable to detect? Add a file or hash here and Simple Antivirus will promptly take action.��6#   5        Wpf.Ui.Controls.Button  5   
   -   5          Name$ AddFile��5      $��1��6%   $
��Left=�65   $��Browse for file��6P   $��	0,66,20,0q�6�   $	��133��6�   $��32��6�   $
��Bold"�6�   ���	#FF2204B8���	#E4FFFFFF   MouseOverBackground �	#80476EE96�     5   
   $
��Wrap=�6   $��20(�6+   $��0,30,0,0q�69   $��Manually add a file hash��6K     Wpf.Ui.Controls.TextBox  5   
   -   5          Name$ AddHashTextBox��5       �� TextWrapping$
 Wrap=�6-   	 �� 	MaxLength$	 64��6A   $
��Left=�6P   
   PlaceholderText$8
 2Enter an SHA1 hash here (40 character hexadecimal)��6k   $��0,5,0,0q�6�     5   
   -   5        $ AddHash��5      $��Add Hash��6%   $	��133��6N   $��32��6Z   $
��Bold"�6f   ���	#FF2204B8���	#E4FFFFFF �	#80476EE9$��0,20,0,0q�6�   5      5      