   M S B A M L   `   `   `  ����M  ITimetableOfClasses, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:TimetableOfClasses.Windows1TimetableOfClasses.Windows1  5   	   5    /TimetableOfClasses.Windows1.LessonReplaceWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ;local)clr-namespace:TimetableOfClasses.Windows1     5   	   -   5        $	��305��5   	   $��335.197��6     � 
ResizeMode$  NoResize=�5
   	   .��5      � $��0,0,0,0q�6   .+�5   
   �� $��0,0,0,0q�6   $
��Left=�6&   $	��327��6A   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��0,10,0,0q�62   .+�5      �� .��6   
День6   �� 5      -   5         ��Name$ DayComboBox��5       �� DisplayMemberPath$
 Name��60   {�.  $	��203��6[   $��72,0,0,0q�6g   5      5      �� 5      $ 
Horizontal=�6   $��0,10,0,0q�62   .+�5      �� .��6   Номер урока6%   �� 5      -   5        $ LessonNumberComboBox��5      {�.  $ Number��6L   $	��203��6g   $��27,0,0,0q�6s   5      5      �� 5      $ 
Horizontal=�6   $��0,10,0,0q�62   .+�5      �� .��6   Номер кабинета6(   �� 5      -   5        $ ClassNumberComboBox��5      $ Number��68   $	��203��6S   $��9,0,0,0q�6_   5      5      �� 5      $ 
Horizontal=�6   $��0,10,0,0q�62   .+�5      �� .��6   Группа6    �� 5      -   5        $ GroupComboBox��5      {�.  $
 Name��6D   $	��203��6]   $��62,0,0,0q�6i   5      5       �� 5!      $ 
Horizontal=�6   $��0,10,0,0q�62   .+�5"      �� .��6   Преподаватель6'   �� 5#      -   5        $ TeacherComboBox��5#      $ Surname��64   $	��203��6�   $��16,0,0,0q�6�   5$      �� 5%      $ 
Horizontal=�6   $��0,10,0,0q�62   .+�5&      �� .��6   Предмет6!   �� 5'      -   5        $ LessonComboBox��5'      $
 Name��63   $	��203��6L   $��52,0,0,0q�6X    J� RenderTransformOrigin$ 0.5,0.5&�6j   z�5(      V� 5)      .~�5*      �� �� 5+       �� AngleY$ 0.043Q�6,   �� 5,      T� 5-       T� Y$ 0.083Q�61   5.      50      51      �� 52      -   5         ��Name$ ClearButton��52      $��Очистить��6*    
  Button#��[  6>   $��176,10,46,0q�6^   $
��Auto��6�   �� 53      -	   5        $ 
SaveButton��53      $��Сохранить��6)   #��[  6>   $��54,-30,176,0q�6^   $
��Auto��6�   54      55      56      