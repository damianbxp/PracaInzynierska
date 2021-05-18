# Program do graficznego programowania robotów przemysłowych oraz ich wizualizacja

## Cel
Celem pracy inżynierskiej stworzenie oprogramowania umożliwiającego graficzne zaprogramowanie robota, export kodu w zależności od wybranego robota, oraz wizualizacja.
## Założenia
Ze względu na obszerność tematu obszar pracy został mocno ograniczony.
#### Etapy prac
1. UI pozwalające na interakcje z programem
2. Postawowe funkcje poruszania ramieniem robota
Jeżeli uda się zrealizować podstawowe cele:
1. Wizualizacja pracy obróbki ubytkowej za pomocą robota przemysłowego
###### Design
Okno programu podzielone na 4 główne obszary:
* Okno dostępnych funkcji
  >Tutaj pojawi się lista wszystkich dostępnych funkcji możliwych do dodania.
* Okno programu
  > W tym oknie znajdzie się lista zawierająca wszystkie bloki programu. Możliwa będzie zmiana kolejności bloków, zaznaczenie ich w celu zmiany właściwości oraz usunięcie.
* Okno właściwości bloku
  > W tym oknie pojawią się parametry aktualnie zaznaczonej funkcji.
* Okno podglądu
  > Tutaj będzie możliwy podgląd pracującego robota

Przy górnej krawędzi znajdzie się toolbar pozwalający na wywołanie innych funkcji oprogramowania:
* Export kodu w języku robota

###### Kod
Oprogramowanie zostanie napisane w C# z wykorzystaniem Unity
Oprogramowanie powinno:
* Posiadać API pozwalające na łatwe dodwawanie nowych funkcji
* Odnajdować roboty na scenie symulatora.
* Odnajdować waypointy na scenie symulatora.


