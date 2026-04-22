# שלב הבנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# העתקה של קובץ הפרויקט מהתיקייה הספציפית
COPY ToDoApi/ToDoApi.csproj ToDoApi/
RUN dotnet restore "ToDoApi/ToDoApi.csproj"

# העתקה של שאר הקבצים
COPY . .
WORKDIR "/src/ToDoApi"
RUN dotnet build "ToDoApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ToDoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# שלב ההרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoApi.dll"]