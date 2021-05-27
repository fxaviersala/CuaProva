# Proves amb Azure ServiceBus

Proves amb dues de les tres llibreries d'Azure per gestionar AzureBus: 

* **CuaProva1**: Fa referència a la llibreria `Azure.Messaging.ServiceBus`. Sembla que és la més nova
* **CuaProva2**: Fa referència a la llibreria `Microsoft.Azure.ServiceBus`

Els projectes estan separats en procés d'enviar i de rebre.

A pesar de les diferències amb els noms de les propietats (**Label**/**Subject**, **ApplicationProperties**/**UserProperties**). En les proves que he fet, qualsevol informació es pot enviar amb una llibreria i recuperar-se amb una llibreria diferent:

```bash
dotnet run --project CuaProva1\
``` 

```bash
C:\> dotnet run --project .\CuaProva2Rebre\
b92f7200dfa849dc980e9fe03f5996d7 - 42 - application/json - usuari
60e3ffaf91674575a97a09ff535d3095 - 43 - application/json - usuari
2a6bfb560e284cf3995a09c7c2b7a6f0 - 44 - application/json - usuari
28552d20737b4769896cf03e45995e46 - 45 - application/json - usuari
aad495a92502422a8fc26c34a3bde46d - 46 - application/json - client
4eb3e6125eb14bd698f87359fbc30875 - 47 - application/json - usuari
02ff15df0fef4e449995c73b1d9b5a7e - 48 - application/json - usuari
9d039120848049f096dab0901658b62d - 49 - application/json - client
996a7d0d75bd467c887d202c41974fdd - 50 - application/json - usuari
fe117ed30fff4a758eefbd0f58b83b77 - 51 - application/json - usuari
45613a5b34d44414a872c48e47ecf8f5 - 52 - application/json - usuari
----------------- Missatges acabats ----------------
b92f7200dfa849dc980e9fe03f5996d7/42 -  remove de la taula usuari : Frederic Pi
60e3ffaf91674575a97a09ff535d3095/43 -  create de la taula usuari : Frederic Pi
2a6bfb560e284cf3995a09c7c2b7a6f0/44 -  create de la taula usuari : Filomenu Garcia
28552d20737b4769896cf03e45995e46/45 -  create de la taula usuari : Manel Puig
aad495a92502422a8fc26c34a3bde46d/46 -  update de la taula client : Mohamed Rius
4eb3e6125eb14bd698f87359fbc30875/47 -  remove de la taula usuari : Newton González
02ff15df0fef4e449995c73b1d9b5a7e/48 -  create de la taula usuari : Bernat Puigpelat
9d039120848049f096dab0901658b62d/49 -  remove de la taula client : Ignasi Roura
996a7d0d75bd467c887d202c41974fdd/50 -  update de la taula usuari : Maria Pinta
fe117ed30fff4a758eefbd0f58b83b77/51 -  remove de la taula usuari : Ramona Reig
45613a5b34d44414a872c48e47ecf8f5/52 -  remove de la taula usuari : Maria de la Asunción Serra
```

## Configuració

He afegit un appsettings.json en l'arrel per si es vol cridar els projectes des de la línia de comandes. Això permet només fer servir el `appsettings.json` de l'arrel:

```bash
dotnet run --project .\CuaProva1\
``` 

En canvi des de VisualStudio cada projecte té el seu. En Visual Studio és millor deixar-los i afegir la configuració en el User-Secrets (tots els projectes comparteixen la clau i per tant tindran la mateixa configuració)
