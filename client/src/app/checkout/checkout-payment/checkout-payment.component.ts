import {Component, Input, OnInit} from '@angular/core';
import {FormGroup} from "@angular/forms";
import { BasketService } from "src/app/basket/basket.service";
import { IBasket } from "src/app/shared/models/basket";
import { IOrder } from "src/app/shared/models/order";
import { CheckoutService } from "src/app/checkout/checkout.service"
import {ToastrService} from "ngx-toastr";
import {error} from "@angular/compiler-cli/src/transformers/util";
import {NavigationExtras, Router} from "@angular/router";

@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss']
})
export class CheckoutPaymentComponent implements OnInit {

  @Input() checkoutForm: FormGroup;
  constructor(private basketService: BasketService,
              private checkoutService: CheckoutService,
              private toastr: ToastrService,
              private router:Router) {
  }

  ngOnInit(): void {
        throw new Error("Method not implemented.");
  }

  submitOrder(){
    const basket = this.basketService.getCurrentBasketValue();
    const orderToCreate = this.getOrderToCreate(basket);
    this.checkoutService.createOrder(orderToCreate).subscribe((order: IOrder)=> {
      this.toastr.success("Order successfully created");
      this.basketService.deleteLocalBasket(basket.id);
      const navigationExtras: NavigationExtras = {state: order};
      this.router.navigate(['checkout/success'], navigationExtras);
      console.log(order);
    }, error => {
      this.toastr.error(error.message);
      console.log(error.message);
    });
  }

  private getOrderToCreate(basket: IBasket) {
    return {
      basketId: basket.id,
      deliveryMethodId: +this.checkoutForm.get('deliveryForm').get('deliveryMethod').value,
      shipToAddress: this.checkoutForm.get('addressForm').value,
    }
  }
}
