import { useEffect, useState } from "react";
import { useOrder } from "../../app/OrderContext";
import { useParams, useLocation } from "react-router-dom";
import { useToken } from "../../app/TokenContext";

export default function Confirmation() {
  const { orderId } = useParams();
  const location = useLocation();
  const { token } = useToken();
  const { orderResult, setOrderResult } = useOrder();

  const [order, setOrder] = useState(
    orderResult || location.state?.order || null,
  );
  useEffect(() => {
    if (!order && token && orderId) {
      fetch(`/api/v1/order/$orderid}`, {
        headers: { Authorixation: `Bearer: ${token}` },
      })
        .then((res) => res.json())
        .then((data) => {
          setOrder(data);
          setOrderResult(data);
        });
    }
  }, [order, orderId, token, setOrderResult]);

  const createdAt = new Date(order.createdAt);
  const pickupTime = new Date(order.pickUpTime);
  return (
    <div>
      <h3>Order number: {order.id}</h3>
      <h1> Hello, {order.customerName}</h1>
      <h2>Your order was placed at {createdAt.toLocaleTimeString()}</h2>
      <h2> Will be ready for pickup at: {pickupTime.toLocaleTimeString()}</h2>
    </div>
  );
}
