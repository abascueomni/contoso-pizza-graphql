import { useEffect } from "react";
import { LoginForm } from "../features/auth/LoginForm";
import { useToken } from "./TokenContext";
import { useOrder } from "./OrderContext";
import { Routes, Route } from "react-router-dom";
import "../css/App.css";
import Order from "../features/order/Order";
import Checkout from "../features/checkout/Checkout";
import Confirmation from "../features/order/Confirmation";
import { useQuery } from "@apollo/client/react";
import type { PizzaInOrder } from "../types/Pizza";
import { GET_PIZZAS } from "../graphql/Pizzas";

function App() {
  const { setMenuItems } = useOrder();
  const { token } = useToken();
  const { data } = useQuery<{ pizzas: PizzaInOrder[] }>(GET_PIZZAS);

  //Logout function
  const LogoutButton = () => {
    const { logout } = useToken();
    return <button onClick={logout}>Logout</button>;
  };

  useEffect(() => {
    if (data?.pizzas) {
      console.log(data.pizzas);
      setMenuItems(data.pizzas);
    }
  }, [data]);

  if (!token) {
    return <LoginForm />;
  }

  return (
    <>
      <div>
        <Routes>
          <Route path="/menu" element={<Order />} />
          <Route path="/checkout" element={<Checkout />} />
          <Route path="/orders/:orderid" element={<Confirmation />} />
          <Route path="*" element={<Order />} />
        </Routes>
      </div>
      <LogoutButton />
    </>
  );
}

export default App;
