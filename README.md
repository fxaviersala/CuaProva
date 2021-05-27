# CuaProva

Proves amb dues de les tres llibreries d'Azure per gestionar AzureBus: 

* CuaProva1: Fa referència a la llibreria `Azure.Messaging.ServiceBus`. Sembla que és la més nova
* CuaProva2: Fa referència a la llibreria `Microsoft.Azure.ServiceBus`

Teòricament qualsevol informació es pot enviar amb una llibreria i recuperar-se amb l'altra sense problemes a pesar dels canvis en els 
noms de les propietats: **Label**/**Subject**, **ApplicationProperties**/**UserProperties**

Els exemples de recepció són lleugerament diferents (CuaProva1 recupera de la cua Dead-letter i CuaProva2 fa un Peek dels missatges abans de recuperar-los)
