   M S B A M L   `   `   `  ����P  LPasswordManagerClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:PasswordManagerClient.ViewsPasswordManagerClient.Views  5      7    1PasswordManagerClient.Views.AddRfidCardDialogView   5      W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:PasswordManagerClient.Views     5      ���	#FFF1F1F1.��5
      �   � RowDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $	��500��6;   $
��Auto��6G   $��10q�6U   .+�5      �� $��Old 6 Digit Pin ��6   $��	10,0,30,0q�60   �� 5      $��New 6 Digit Pin��6   5      �� 5   
   $��1��6   $ 
Horizontal=�6"   $	��500��6;   $
��Auto��6G   .+�5      �� $��10q�6   $,��&pack://application:,,,/Fonts/#password)�6"   $��20��6�   $	��100��6�   $
��Left=�6�   ��6V   �� *
RFIDOldPin+ �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5       $,��&pack://application:,,,/Fonts/#password)�6   $��20��6�   $	��100��6�   $
��Left=�6�   ��6J   �� *	RFIDPin+$ PropertyChanged=�6�   �� 5"      $��10q�6�   $	��100��6�   ��6   ��  �� RelativeSource ��  �� AncestorType
 Window��6f   �� *ScanRfidCommand+.��6�   	Scan RFID6�   5#      ]� 5%   
   $��3��6   .+�5&      �� $
��Left=�6�   $��10q�6�   $��32��6�   $	��220��6�   ��6   ��  �� 
 Window��6f   �� *CancelCommand+.��6�   Cancel6�   5'      5)      5*      