import { type PizzaInOrder } from "./Pizza";
import { type DrinkQuantity } from "./Drink";
export interface OrderResponse {
    id:number;
    customerName:string;
    createdAt:string;
    pickUpTime:string;
    pizzas: PizzaInOrder[];
    drinks: DrinkQuantity[];
    totalPrice:number;
}