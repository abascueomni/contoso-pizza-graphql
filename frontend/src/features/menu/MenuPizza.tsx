import type { PizzaDto } from "../../types/PizzaDto";

interface MenuPizzaProps {
  pizza: PizzaDto;
  quantity: number;
  onQuantityChange: (pizzaId: number, quantity: number) => void;
}
export default function MenuPizza({
  pizza,
  quantity,
  onQuantityChange,
}: MenuPizzaProps) {
  return (
    <div className="row align-items-center border rounded my-2 p-2">
      <div className="col-md-6">
        <h4 className="mb-1">{pizza.name}</h4>
        <small className="text-muted">
          Toppings: {pizza.toppings.join(", ")}
        </small>
      </div>
      <div className="col-md-3 text-center">
        <strong>${pizza.price.toFixed(2)}</strong>
      </div>
      <div className="col-md-3 text-center">
        <input
          type="number"
          min={0}
          value={quantity}
          className="form-control"
          onChange={(e) =>
            onQuantityChange(pizza.id, parseInt(e.target.value) || 0)
          }
        />
      </div>
    </div>
  );
}
