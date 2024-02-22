# Symulator Obróbki Skrawaniem z Wykorzystaniem Robota Przemysłowego

## Opis Projektu
Projekt został opracowany w ramach pracy inżynierskiej i ma na celu stworzenie programu symulującego obróbkę skrawaniem z wykorzystaniem robota przemysłowego. Jako model przykładowy wybrano model robota KUKA KR60 HA. W ramach projektu zaimplementowano algorytm kinematyki odwrotnej, dzięki czemu robot jest w stanie podążać za wyznaczonym punktem. Użytkownik ma możliwość sterowania ruchem robota za pomocą kodu G-code lub manualnie.

## Funkcje
- Sterowanie ruchem robota za pomocą kodu G-code lub ręcznie.
- Wykorzystanie modelu robota KUKA KR60 HA.
- Proceduralne generowanie półfabrykatu w postaci prostopadłościanu.
- Implementacja algorytmu "Marching Cubes" do generowania efektów symulacji skrawania w czasie rzeczywistym.
- Wsparcie dla komend G0, G1, G2, G3.
- Wbudowany edytor tekstowy umożliwiający pisanie i modyfikację programu obróbki.
- Wyświetlanie aktualnie wykonywanej komendy w oknie konsoli.
