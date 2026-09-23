# Amazing Banking App — UI

React + Vite + TypeScript frontend with Tailwind CSS. Consumes the Amazings API Customer endpoints.

## Prerequisites

- Node.js 20+
- Amazings API running locally (default `http://localhost:5242`)

## Setup

```powershell
cd UI
npm install
```

Copy `.env.example` to `.env.development` if needed. Default:

```text
VITE_API_BASE_URL=http://localhost:5242
```

## Run

```powershell
npm run dev
```

Open `http://localhost:5173`.

Start the API first (from repo root):

```powershell
cd "Amazings API"
dotnet run --launch-profile http
```

## Build

```powershell
npm run build
```
