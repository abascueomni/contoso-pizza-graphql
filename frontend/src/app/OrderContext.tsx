import React, {
  createContext,
  useContext,
  useState,
  type ReactNode,
} from "react";
import { type PizzaInOrder } from "../types/Pizza";
import { type DrinkQuantity } from "../types/Drink";
import { type OrderResponse } from "../types/Order";

interface OrderContextType {
  menuItems: PizzaInOrder[];
  setMenuItems: (items: PizzaInOrder[]) => void;

  quantities: Record<number, number>;
  setQuantities: React.Dispatch<React.SetStateAction<Record<number, number>>>;

  drinks: DrinkQuantity[];
  setDrinks: React.Dispatch<React.SetStateAction<DrinkQuantity[]>>;

  orderResult: OrderResponse | null;
  setOrderResult: React.Dispatch<React.SetStateAction<OrderResponse | null>>;

  subtotal: () => number;
}

const OrderContext = createContext<OrderContextType | undefined>(undefined);

interface OrderProviderProps {
  children: ReactNode;
}

export const OrderProvider = ({ children }: OrderProviderProps) => {
  const [menuItems, setMenuItems] = useState<PizzaInOrder[]>([]);
  const [quantities, setQuantities] = useState<Record<number, number>>({});
  const [drinks, setDrinks] = useState<DrinkQuantity[]>([]);
  const [orderResult, setOrderResult] = useState<OrderResponse | null>(null);

  const subtotal = () => {
    const pizzasTotal = menuItems.reduce((sum, pizza) => {
      const qty = quantities[pizza.id] || 0;
      return sum + pizza.price * qty;
    }, 0);

    const drinksTotal = drinks.reduce((sum, d) => sum + d.quantity * 2, 0);

    return pizzasTotal + drinksTotal;
  };

  return (
    <OrderContext.Provider
      value={{
        menuItems,
        setMenuItems,
        quantities,
        setQuantities,
        drinks,
        setDrinks,
        orderResult,
        setOrderResult,
        subtotal,
      }}
    >
      {children}
    </OrderContext.Provider>
  );
};

export const useOrder = () => {
  const context = useContext(OrderContext);
  if (!context) {
    throw new Error("useOrder must be used within an OrderProvider");
  }
  return context;
};
