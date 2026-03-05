import type { PizzaTopping } from "./Topping";

export interface PizzaInOrderDto
{
    id:number;
    name:string;
    price: number;
    isGlutenFree: boolean;
    isMenuPizza:boolean;
    toppings:PizzaTopping[];
    quantity :number;
}