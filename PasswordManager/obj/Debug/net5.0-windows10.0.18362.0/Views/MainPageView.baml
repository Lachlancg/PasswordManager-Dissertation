   M S B A M L   `   `   `  ����P  LPasswordManagerClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:PasswordManagerClient.ViewsPasswordManagerClient.Views  5      S.clr-namespace:PasswordManagerClient.ViewModels PasswordManagerClient.ViewModels  5
      .    (PasswordManagerClient.Views.MainPageView   5      \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:PasswordManagerClient.Views     5      D @ModernWpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM IModernWpf.Controls, Version=0.9.3.0, Culture=neutral, PublicKeyToken=null,ui!http://schemas.modernwpf.com/2019   5	      A
viewModels.clr-namespace:PasswordManagerClient.ViewModels   5
      ���	#FFE6E6E6  � 	Resources  5         VisBool��%   &	        �� 5   
   6'   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��0 �6   �� 5      $	��640 �6   .+�5   
   � $��3��6    5      �� 5       $��* �6#   .+�5#      o� $	��Top=�6   ���	#FF4A5356.@�5$      m� 
�������$
��File��6.   .@�5%      m� $��Add Category��6@    m� Command 6    �� *AddCategory+6@   m� 5&      $��Delete Category��6C    6    �� *DeleteCategory+6C   m� 5'      $��Log Out��5(   P    m� CommandParameter 5'      ��  6<   �� *DataContext.LogOutCommand+ �� RelativeSource ��  �� AncestorType0   *PasswordManagerClient.Views.MainWindowView"   �� Mode$ OneWay=�5(   P   5)      m� 5+      
�������$��Tools��6.   .@�5,      m� $��Generate Password��6D    6   �� *GeneratePassword+6D   m� 5-      $��Change Password��6B    6   �� *ChangePassword+6B   m� 5.      $��Change RFID Card��6>    6   �� *
ChangeRfid+6>   5/      m� 50      
�������$
��Sync��6.   .@�51      m� $��	Go Online��52   P    51       ��  6=   �� *DataContext.GoOnlineCommand+ �� "  $ OneWay=�52   P   m� 53      $
��Sync��6@    6    �� *SyncCommand+6@   m� 54      $��Delete Account��55   `    54       ��  6=   �� *" DataContext.DeleteAccountCommand+ �� "  $ OneWay=�55   `   56      57      5;      � 5>   
   $��1��6    5?      �� 5@      $
��Auto �6#   �� 5A      $	��320 �6#   �� 5B      $	��640 �6#   .+�5D      '  !ModernWpf.Controls.NavigationView  -   5          Name$ NavigationView��5D       $��1��68   ���	#FF6B8891$��False��5E   7   	   PaneDisplayMode$
	 Left=�6I   
   IsSettingsVisible$
 False��6�      IsBackButtonVisible$ 	Collapsed=�6�      SelectedItem 6`   �� *NavSelectedItem+$ TwoWay=�   MenuItemsSource 6�   �� *NavigationData+   MenuItemTemplate 5F      �� 5G      .R�5H      +  %ModernWpf.Controls.NavigationViewItem  ��60   �� *Name+   Icon 5I      #  ModernWpf.Controls.SymbolIcon  5J   "      Symbol 60   �� *Symbol+5L      5N      .��5Q      �� -   5         ��Name$ NavigationBorder��5Q   !   ��6;   �� *	NavItem+6Y   5R      5T      5U      5V      