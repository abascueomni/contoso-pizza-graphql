export interface PizzaDto {
    id: number;
    name:string;
    price: number;
    isGlutenFree: boolean;
    isMenuPizza: boolean;
    toppings:string[];
}