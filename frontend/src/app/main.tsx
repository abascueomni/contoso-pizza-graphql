import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "../css/index.css";
import App from "./App.tsx";
import { TokenProvider } from "./TokenContext.tsx";
import "bootstrap/dist/css/bootstrap.min.css";
import { BrowserRouter } from "react-router-dom";
import { OrderProvider } from "./OrderContext.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <TokenProvider>
      <OrderProvider>
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </OrderProvider>
    </TokenProvider>
  </StrictMode>,
);
