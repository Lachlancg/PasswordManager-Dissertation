   M S B A M L   `   `   `  ����P  LPasswordManagerClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:PasswordManagerClient.ViewsPasswordManagerClient.Views  5      2    ,PasswordManagerClient.Views.AllItemsPageView   5      W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:PasswordManagerClient.Views     5      D @ModernWpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM IModernWpf.Controls, Version=0.9.3.0, Culture=neutral, PublicKeyToken=null,ui!http://schemas.modernwpf.com/2019   5      Ainteractivity+http://schemas.microsoft.com/xaml/behaviors   64   ���	#FFE6E6E6.��5      �   � RowDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $	��320 �6   �� 5      $	��320 �6   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$ 	searchBox��5      $��0��6   $	��Top=�5      $��12q�5       � ToolTip$ Search��5   O   ��6   �� *SearchQuery+ �� UpdateSourceTrigger$ PropertyChanged=�.��6`   5      �� 5    
   $��1��6   $	��288��6=   $��28��6I   $��32,0,0,0q�6U   ��6!   �� *	AddSite+.��6g   Add6l   �� 5#   
   -   5         ��Name$ 	SitesView��5#   [   $��1��6   $��0��6!   
������� �� SelectedItem 61   �� *SiteSelectedItem+��6�   �� *
SiteData+��5$      �� 5%      .R�5&      �  � Orientation$ Vertical=�6    .+�5'      �� $
��Bold"�6:   ��6$   �� *Name+6:   �� 5)      ��6$   �� *Domain+5*      5+      5.      � 50   
   $��1��6   $��1��6    51      �� 52      $��* �6#     55      �� 56      $
��Auto �6    �� 57      $
��Auto �6    �� 58      $
��Auto �6    �� 59      $
��Auto �6    �� 5:      $
��Auto �6    �� 5;      $
��Auto �6    .+�5>      �� -   5         ��Name$ TextBoxScrollViewer��5>      $��32,0,0,0q�6�   y�68   �� *TextBoxVisibility+$ PropertyChanged=�.��5?      )  #ModernWpf.Controls.SimpleStackPanel  .+�5@      �   5A      �� 5B      $
��Auto �6,   �� 5C      $
��Auto �6,    5E      �� 5F      $	��240 �6/   6=   �� 5G      $
��Auto �6/   6>   .+�5I      �� $��0��6$   $
��Name��64   �� 5J      -   5        $ NameTextBox��5J   "   $��0��67   $��1��6n   ��6G   �� *SiteSelectedItem.Name+6~   �� 5K      $ 	Favourite��6#   $��1��6h   $��1��6u   $��18,0,0,0q�6�   	 d� 	IsChecked	 67   �� *SiteSelectedItem.Favourite+6�   5L      �� 5M      $��1��6    $��Username��60   �� 5N      -   5        $ UsernameTextBox��5N      $��2��6b   $��1��6o   ��67   �� *SiteSelectedItem.Username+6�   � 5O        5P      �� 5Q      $
��Auto �6,   �� 5R      $
��Auto �6,    5T      �� 5U      $	��240 �6/   6=   �� 5V      $
��Auto �6/   6>   .+�5X      �� $��0��6$   $��Password��64   �� 5Y      -   5        $ PasswordTextBox��5Y   "   $��1��6�   $��0��6�   
 �� 
IsReadOnly
 6;   �� *HidePasswordAndReadonly+��6j   �� *
Password+6�   �� 5Z      $ Show Password��6#   $��1��6i   $��1��6v   $��18,0,0,0q�6�   	 6;   �� *HidePasswordAndReadonly+6�   5[      �� 5\      $��1��6    $��Domain��60   �� 5]      -   5        $ NotesTextBox��5]      $��4��6]   $��1��6j   ��64   �� *SiteSelectedItem.Domain+6|   �� 5^      $��1��6    $��Notes��60   �� 5_      -   5        $ DomainTextBox��5_      $��4��6]   $��1��6j   $	��128��6z    �� TextWrapping$
 Wrap=�6�    �� AcceptsReturn$
 True��6�   ��65   �� *SiteSelectedItem.Notes+6�   �� 5`      $��1��6    $��Category��60   �� 5a      $��5��6w   $��0��6�   $��Center=�6�   $��Center=�6�   $	��288��6�   ��6   �� *CategoryData+ 6D   �� *CategoryItem+ �� Mode$ TwoWay=�6�   �� 5d      $��0,32,0,0q�6   $��Center=�6M   $	��288��6j   $��28��6v   ��60   �� *
SaveSite+.��6�   Save6�   �� 5e      $��0,32,0,0q�6   $��Center=�6N   $	��288��6k   $��28��6w   ��6/   �� *
DeleteSite+.��6�   Delete6�   5g      5i      5l      5n      5q      