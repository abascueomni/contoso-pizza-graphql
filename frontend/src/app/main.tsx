// External Libraries
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css"; // Global styles
// Apollo Client
import { ApolloClient, InMemoryCache, HttpLink } from "@apollo/client";
import { SetContextLink } from "@apollo/client/link/context";
import { ApolloProvider } from "@apollo/client/react";
// Internal Components & Contexts
import App from "./App.tsx";
import { TokenProvider } from "./TokenContext.tsx";
import { OrderProvider } from "./OrderContext.tsx";

// Styles
import "../css/index.css"; // Local styles

const httpLink = new HttpLink({
  uri: "http://10.0.0.238:5000/gql",
});

const authLink = new SetContextLink(() => {
  const token = localStorage.getItem("token"); // I cannot seem to get useToken to work here at all
  return {
    headers: {
      authorization: token ? `Bearer ${token}` : "",
    },
  };
});

const client = new ApolloClient({
  link: authLink.concat(httpLink),
  cache: new InMemoryCache(),
});

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ApolloProvider client={client}>
      <TokenProvider>
        <OrderProvider>
          <BrowserRouter>
            <App />
          </BrowserRouter>
        </OrderProvider>
      </TokenProvider>
    </ApolloProvider>
  </StrictMode>,
);
