# Symulacja obróbki skrawaniem robotami przemysłowymi
## Cel
- Symulowany robot wykonuje ścieżkę narzędzia zaprogramowaną w gcode-dzie.
- Przemieszczające się narzędzie pozostawia ślad w półfabrykacie.

## Robot
- Wybrany model robota: KUKA KR60 HA
- Kożystając z kinematyki odwrotnej robot podąża za punktem
- Ruch Jog pozwalający na ręczne sterowanie robotem

## Półfabrykat
- Prostopadłościenny blok o zadanych wymiarach
- Umieszczony w określonym położeniu
- Generowany algorytmem "Marching Cubes"

## Gcode
- Do programu zaimplementowane zostaną podstawowe funkcje Gcode m.in. G0,G1,G2,G3
- Wbudowany edytor tekstowy pozwoli na napisanie oraz modyfikację programu
- Obecnie wykonywana komenda wyświetlana jest w oknie konsoli
- Wykryte błędy użytkownika wyświetlane bedą w konsoli
