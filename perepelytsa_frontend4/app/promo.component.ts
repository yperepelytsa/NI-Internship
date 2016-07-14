import { Component, Input } from '@angular/core';
import { Item } from './item';
@Component({
  selector: 'my-promo',
  template: `			
     Retail price: {{countsum(itemsinp)}}$ <br>
     Promocode:<br>
     <input [(ngModel)]="promo" class="input-promo bg-10"/><br>
     <div [ngSwitch]="promo">
     <div *ngSwitchCase="'ni16%'">Discount: 16%</div>
     <div *ngSwitchCase="'ni33%'">Discount: 33%</div>
     <div *ngSwitchCase="'ni10$'">Discount: 10$</div>
     <div *ngSwitchDefault></div>
    </div>
     <b>Total: {{counttotal(itemsinp)}}$</b><br>
     <button type="button" id="checkout" class="btn btn-default btn-sm">Checkout</button>
  `})
  export class PromoComponent {
      @Input()
      itemsinp: Item[];
      promo: string;
      
       countsum(inp:Item[]){
           var res:number=0;          
           inp.forEach(element => {
               res+=element.quantity*element.itemPrice;
           });
return res;
      }

      counttotal(){
          if(this.promo==null)
          return this.countsum(this.itemsinp);
          if(this.promo.includes("ni16%")){
          return Math.round(this.countsum(this.itemsinp)*0.84);
          }
          if(this.promo.includes("ni33%")){
          return Math.round(this.countsum(this.itemsinp)*0.67);
         }
          if(this.promo.includes("ni10$")){
              if(this.countsum(this.itemsinp)>10)
          return this.countsum(this.itemsinp)-10;
          else return 0;
         }
          return this.countsum(this.itemsinp);
  }
  }