﻿FROM mcr.microsoft.com/dotnet/core/sdk:3.1
COPY bin/Release/netcoreapp3.1/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "BattleShip.Web.dll"]