# שלב 1: בנייה (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

# העתקת קובץ הפרויקט וביצוע Restore
COPY ToDoApi/ToDoApi.csproj ToDoApi/
RUN dotnet restore "ToDoApi/ToDoApi.csproj"

# העתקת כל שאר הקבצים ובנייה
COPY . .
WORKDIR "/src/ToDoApi"
RUN dotnet publish "ToDoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# שלב 2: הרצה (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
# העתקה מהשלב הקודם (build-env)
COPY --from=build-env /app/publish .

# הגדרות פורט עבור Render
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ToDoApi.dll"]