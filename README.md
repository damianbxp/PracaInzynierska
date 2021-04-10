# Program do graficznego programowania robotów przemysłowych oraz ich symulacja z wykorzystaniem Webots

## Cel
Celem pracy inżynierskiej stworzenie oprogramowania umożliwiającego graficzne zaprogramowanie robota, export kodu w zależności od wybranego robota, oraz symulacja zaprogramowanego robota w Webot’sie.
## Założenia
Ze względu na obszerność tematu obszar pracy został mocno ograniczony.
#### Oprogramowanie
###### Design
Okno programu podzielone na 3 główne obszary:
* Okno dostępnych funkcji
  >Tutaj pojawi się lista wszystkich dostępnych funkcji możliwych do dodania.
* Okno programu
  > W tym oknie znajdzie się lista zawierająca wszystkie bloki programu. Możliwa będzie zmiana kolejności bloków, zaznaczenie ich w celu zmiany właściwości oraz usunięcie.
* Okno właściwości bloku
  > W tym oknie pojawią się parametry aktualnie zaznaczonej funkcji.

Przy górnej krawędzi znajdzie się toolbar pozwalający na wywołanie innych funkcji oprogramowania:
* Export kodu w języku robota

###### Kod
Oprogramowanie zostanie napisane w C# z wykorzystaniem WPF
Oprogramowanie powinno:
* Posiadać API pozwalające na łatwe dodwawanie nowych funkcji
* Odnajdować roboty na scenie symulatora.
* Odnajdować waypointy na scenie symulatora.
#### Symulator
Do symulacji zostanie wykorzystany Webots. Jest on darmowy i uniwersalny.

Symulator umożliwi wstawienie robota oraz waypointów do sceny.
Konieczne będzie zaprogramowanie robotów aby wykonywały polecenia przesyłane przez oprogramowanie.