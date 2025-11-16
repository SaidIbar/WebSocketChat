<h2>C# console chat applikation</h2>
Applikationen startar med att användaren får en meny vall med 8 val
<ul>
<li>1- Man kan koppla sig med servern och starta chatten med att ange först ett använda namn och att användaren kan välja att koppla sig till ett 
  specifikt rum och börja chatta.</li>
<li>2.Användaren kan koppla ifrån servern, ett meddelandet skickas om att användaren har lämnat rummet och med det lämnat chatten.</li> 
<li>3. Användaren kan lista alla meddelandehistorik</li>
<li>4. Användaren kan lista alla serverns aktuella events.</li>
<li>5. Användaren får möjlighet att ändra Event Namnet.</li>
<li>6. Användaren kan lista alla meddelanden som inte är kompatibla med ens objekt.</li>
<li>7.Användaren kan koppla ifrån servern</li>
<li>8. Användaren lämnar applikationen.</li>
</ul>

I applicationen skickas Json formaterade meddelanden och tar emot Json och presenteras på ett läsbart sätt, meddelanden som inte kan formateras sammlas i en lista och presenteras i dess form.
Help klassen <b>"ClientManagerHelpers"</b> används för att formater och validera texter och datumen.
When disconnected the application will do a number of attempt to reconnect.
