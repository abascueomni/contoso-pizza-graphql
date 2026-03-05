import MenuPizza from "../menu/MenuPizza";
import { useOrder } from "../../app/OrderContext";
import { useNavigate } from "react-router-dom";

export default function Order() {
  const { menuItems, quantities, setQuantities, subtotal } = useOrder();
  const navigate = useNavigate();

  const handleQuantityChange = (pizzaId: number, qty: number) => {
    setQuantities((prev) => ({
      ...prev,
      [pizzaId]: qty,
    }));
  };
  const handleClick = () => {
    navigate("/checkout");
  };

  return (
    <div className="container my-4">
      <div className="row">
        <div className="col">
          <h1>Menu items</h1>
          {menuItems.map((p) => (
            <MenuPizza
              key={p.id}
              pizza={p}
              quantity={quantities[p.id] || 0}
              onQuantityChange={handleQuantityChange}
            />
          ))}
          <div>Subtotal: ${subtotal().toFixed(2)}</div>
          <button className="btn btn-primary" onClick={handleClick}>
            Checkout
          </button>
        </div>
      </div>
    </div>
  );
}
