   M S B A M L   `   `   `  ����P  LPasswordManagerClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:PasswordManagerClient.ViewsPasswordManagerClient.Views  5      4    .PasswordManagerClient.Views.GoOnlineDialogView   5      W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:PasswordManagerClient.Views     5      ���#C5C6C7.��5
      �   � RowDefinitions  5   
   �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6     � ColumnDefinitions 5      �� 5      $
��Auto �6#   .+�5      �� $	��200��6   $
��Auto��6   .+�5      �� $��Email��6   $��0,50,0,0q�6%   69   �� 5      $	��200��6Z   ��6   �� *EmailAddress+ �� UpdateSourceTrigger$ PropertyChanged=�6Z   �� 5      $��Master Password��6   65   �� 5      $,��&pack://application:,,,/Fonts/#password)�6   $	��200��6�   ��6K   �� *
Password+$ PropertyChanged=�6�   5      �� 5   
   $��3��6   $��RFID 6 Digit Pin��6!   $��
150,10,0,0q�69   �� 5   
   $��4��6    �� Orientation$ 
Horizontal=�6"   $	��200��6;   $
��Auto��6G   .+�5       �� $,��&pack://application:,,,/Fonts/#password)�6   $��20��6�   $	��100��6�   $
��Left=�6�   ��6J   �� *	RFIDPin+$ PropertyChanged=�6�   �� 5!      $��	0,10,0,10q�6y   $	��100��6�   ��6   �� *
RfidColour+��67   �� ��6T   �� *LoadRfidCommand+.��6�   	Load RFID6�   5"      ]� 5$   
   $��5��6   .+�5%      �� $
��Left=�6�   $��10q�6�   $��32��6�   $	��220��6�   ��6   ��  �� RelativeSource ��  �� AncestorType
 Window��6f   �� *CancelCommand+.��6�   Cancel6�   �� 5&       �� 	IsDefault$
 True��6   $��Right=�6�   $��10q�6�   $��32��6�   $	��240��6�   ��6&   ��  �� 
 Window��6w   �� *CopyCommand+.��6�   Login6�   5'      5)      5*      