
1 - battery
"bibliotek" for event sourcing
opprette, endre og slette domeneobjekter - alt lagres som events
kan hente historiske tilstander
bonus? - ting som interagerer. hvor mange av hver frukt har man på hvert tidspunkt.


2 - 
funker bra, men nå begynner det å bli mange events.
går fint å hente alt for en spesifikk frukt. men hva med aggregering på tvers av frukt.
Hente "gi meg alle frukt at: datetime" betyr å lese alle events i databasen og bygge opp fra bunn.
cqrs:
	ta vare på feks siste versjon av hver frukt til oversiktssiden. reberegne når en frukt endrer seg
	hvis man skal se på historikken av en enkeltfrukt så kan det fortsatt beregnes
	klienten får ikke alltid mest oppdaterte tilstand, eventual consistency

3 - cqrs er mer komplekst enn å ikke ha det. 
Må sørge for at lesemodellene oppdateres, uten at dette skal merkes av de som produserer events
Kan også være andre sideeffekter av at noe oppdateres - skal si ifra til eksterne systemer, publisere til datakatalog, etc
Fint å gjøre det dette med mediator-pattern (men ikke med mediatR)

Kan løses ved å ha noe separat som poller for å se om det har kommet nye events, og oppdaterer lesemodellene
Raskere resolution og mindre waste av å trigges av eventene,
	men da må man være trygg på at denne oppdateringen skjer selv om applikasjonen din feks crasher
Naturlig konklusjon å bygge dette oppå en slags buss, a la mass transit eller nservicebus. oppdateringen og propageringen skjer asynkront


