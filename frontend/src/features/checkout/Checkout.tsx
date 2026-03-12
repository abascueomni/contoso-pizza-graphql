import { useState } from "react";
import { useOrder } from "../../app/OrderContext";
import { useNavigate } from "react-router-dom";
import { useMutation } from "@apollo/client/react";
import type {
  CreateOrderResult,
  CreateOrderVariables,
} from "../../types/Order";
import { CREATE_ORDER } from "../../graphql/Orders";

//Checkout page for reviewing and submitting a pizza order
export default function Checkout() {
  const [name, setName] = useState("");
  const { quantities, menuItems, setOrderResult } = useOrder();
  const navigate = useNavigate();
  const [createOrder] = useMutation<CreateOrderResult, CreateOrderVariables>(
    CREATE_ORDER,
  );

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

    try {
      const { data } = await createOrder({
        variables: {
          input: payload,
        },
      });

      if (data) {
        const result = data.createOrder;
        console.log("Order Success", result);
        setOrderResult(result);
        navigate(`/orders/${result.id}`);
      } else {
        console.error("Mutation failed: No data returned.");
      }
    } catch (err) {
      console.error(err);
    }
  };

  //function which calculates subtotal for pizzas befor esubmitting to server
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
