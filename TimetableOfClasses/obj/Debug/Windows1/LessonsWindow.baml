   M S B A M L   `   `   `  ����M  ITimetableOfClasses, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:TimetableOfClasses.Windows1TimetableOfClasses.Windows1  5   	   /    )TimetableOfClasses.Windows1.LessonsWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ;local)clr-namespace:TimetableOfClasses.Windows1     5   	   -   5        $	��305��5   	   $��335.197��6     � 
ResizeMode$  NoResize=�5
   	   .��5      � $��0,0,0,0q�6   .+�5   
   �� $��0,0,0,0q�6   $
��Left=�6&   $	��327��6A   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��0,10,0,0q�62   .+�5      �� .��6   
День6   �� 5      -   5         ��Name$ DayComboBox��5       �� DisplayMemberPath$
 Name��60   $	��203��6I   $��72,0,0,0q�6U   5      5      �� 5      $ 
Horizontal=�6   $��0,10,0,0q�62   .+�5      �� .��6   Номер урока6%   �� 5      -   5        $ LessonNumberComboBox��5      $ Number��69   $	��203��6T   $��27,0,0,0q�6`   5      5      �� 5      $ 
Horizontal=�6   $��0,10,0,0q�62   .+�5      �� .��6   Номер кабинета6(   �� 5      -   5        $ ClassNumberComboBox��5      $ Number��68   $	��203��6S   $��9,0,0,0q�6_   5      5      �� 5      $ 
Horizontal=�6   $��0,10,0,0q�62   .+�5      �� .��6   Группа6    �� 5      -   5        $ GroupComboBox��5      $
 Name��62   $	��203��6K   $��62,0,0,0q�6W   5      5       �� 5!      $ 
Horizontal=�6   $��0,10,0,0q�62   .+�5"      �� .��6   Преподаватель6'   �� 5#      -   5        $ TeacherComboBox��5#      $ Surname��64   $	��203��6�   $��16,0,0,0q�6�   5$      �� 5%      $ 
Horizontal=�6   $��0,10,0,0q�62   .+�5&      �� .��6   Предмет6!   �� 5'      -   5        $ LessonComboBox��5'      $
 Name��63   $	��203��6L   $��52,0,0,0q�6X    J� RenderTransformOrigin$ 0.5,0.5&�6j   z�5(      V� 5)      .~�5*      �� �� 5+       �� AngleY$ 0.043Q�6,   �� 5,      T� 5-       T� Y$ 0.083Q�61   5.      50      51      �� 52      -   5         ��Name$ 
SaveButton��52      $��Сохранить��6)    
  Button#��[  6>   $��114,10,96,0q�6^   $
��Auto��6�   53      54      55      