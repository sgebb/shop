
1 - battery
"bibliotek" for event sourcing
opprette, endre og slette domeneobjekter - alt lagres som events
kan hente historiske tilstander
bonus? - ting som interagerer. hvor mange av hver frukt har man p� hvert tidspunkt.


2 - 
funker bra, men n� begynner det � bli mange events.
g�r fint � hente alt for en spesifikk frukt. men hva med aggregering p� tvers av frukt.
Hente "gi meg alle frukt at: datetime" betyr � lese alle events i databasen og bygge opp fra bunn.
cqrs:
	ta vare p� feks siste versjon av hver frukt til oversiktssiden. reberegne n�r en frukt endrer seg
	hvis man skal se p� historikken av en enkeltfrukt s� kan det fortsatt beregnes
	klienten f�r ikke alltid mest oppdaterte tilstand, eventual consistency

3 - cqrs er mer komplekst enn � ikke ha det. 
M� s�rge for at lesemodellene oppdateres, uten at dette skal merkes av de som produserer events
Kan ogs� v�re andre sideeffekter av at noe oppdateres - skal si ifra til eksterne systemer, publisere til datakatalog, etc
Fint � gj�re det dette med mediator-pattern (men ikke med mediatR)

Kan l�ses ved � ha noe separat som poller for � se om det har kommet nye events, og oppdaterer lesemodellene
Raskere resolution og mindre waste av � trigges av eventene,
	men da m� man v�re trygg p� at denne oppdateringen skjer selv om applikasjonen din feks crasher
Naturlig konklusjon � bygge dette opp� en slags buss, a la mass transit eller nservicebus. oppdateringen og propageringen skjer asynkront


