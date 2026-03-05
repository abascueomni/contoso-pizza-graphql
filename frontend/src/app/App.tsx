import { useEffect } from "react";
import { LoginForm } from "../features/auth/LoginForm";
import { useToken } from "./TokenContext";
import { useOrder } from "./OrderContext";
import { Routes, Route } from "react-router-dom";
import "../css/App.css";
import Order from "../features/order/Order";
import Checkout from "../features/checkout/Checkout";
import Confirmation from "../features/order/Confirmation";

function App() {
  const { setMenuItems } = useOrder();
  const { token } = useToken();

  //Logout function
  const LogoutButton = () => {
    const { logout } = useToken();
    return <button onClick={logout}>Logout</button>;
  };
  const API_BASE_URL: string =
    import.meta.env.VITE_API_URL || "http://localhost:5000";

  //Get the Pizza menu
  const getPizzas = async () => {
    if (!token) return;
    const res = await fetch(`${API_BASE_URL}/api/v1/pizza`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();
    setMenuItems(data);
  };

  useEffect(() => {
    //console.log("App mounted or token changed", token);
    getPizzas();
  }, [token]);

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
