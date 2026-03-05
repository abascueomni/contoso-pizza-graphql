import { useState } from "react";
import { useToken } from "../../app/TokenContext";
import { useOrder } from "../../app/OrderContext";
import { useNavigate } from "react-router-dom";

export default function Checkout() {
  const [name, setName] = useState("");
  const { token } = useToken();
  const { quantities, menuItems, setOrderResult } = useOrder();
  const navigate = useNavigate();

  const handlePlaceOrder = async () => {
    const pizzasToOrder = menuItems
      .filter((p) => quantities[p.id] > 0)
      .map((p) => ({
        pizzaId: p.id,
        quantity: quantities[p.id],
      }));

    const payload = {
      customerName: name,
      pizzas: pizzasToOrder,
      drinks: [],
      coupon: null,
    };
    console.log(JSON.stringify(payload, null, 2));
    const API_BASE_URL: string =
      import.meta.env.VITE_API_URL || "http://localhost:5000";
    try {
      const res = await fetch(`${API_BASE_URL}/api/v2/orders`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(payload),
      });

      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      const result = await res.json();
      console.log("Order Success", result);
      setOrderResult(result);
      navigate(`/orders/${result.id}`);
    } catch (err) {
      console.error(err);
    }
  };

  const subtotal = menuItems.reduce((sum, pizza) => {
    const qty = quantities[pizza.id] || 0;
    return sum + pizza.price * qty;
  }, 0);

  return (
    <div>
      {menuItems
        .filter((p) => (quantities[p.id] || 0) > 0)
        .map((p) => (
          <p key={p.id}>
            You donkey you wanted {quantities[p.id] || 0} {p.name} pizzas
          </p>
        ))}
      <h2> Your subtotal is ${subtotal.toFixed(2)}</h2>
      <div>
        <input
          type="text"
          value={name}
          placeholder="Name for Order"
          className="form-control"
          onChange={(e) => setName(e.target.value)}
        ></input>
      </div>
      <div>
        <button onClick={handlePlaceOrder} disabled={!name}>
          Place Order
        </button>
      </div>
    </div>
  );
}
