"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('@angular/core');
var TableComponent = (function () {
    function TableComponent() {
    }
    TableComponent.prototype.decrement = function (item) {
        if (item.quantity > 0)
            item.quantity--;
    };
    TableComponent.prototype.removeit = function (item) {
        this.itemsinp.splice(this.itemsinp.indexOf(item), 1);
    };
    TableComponent.prototype.increment = function (item) {
        item.quantity++;
    };
    __decorate([
        core_1.Input(), 
        __metadata('design:type', Array)
    ], TableComponent.prototype, "itemsinp", void 0);
    TableComponent = __decorate([
        core_1.Component({
            selector: 'my-table',
            template: "  \n  <table class=\"table shop-cart\">\n  <thead>\n  <tr>\n  <th style=\"width:61%\"><b class=\"bg-4\">Item name</b></th>\n  <th style=\"width:13%\"><b class=\"bg-4\">Price</b></th>\n  <th style=\"width:13%\"><b class=\"bg-4\">Quantity</b></th>\n  <th style=\"width:13%\"><b class=\"bg-4\">Total</b></th>\n  </tr>\n  </thead>\n  <tbody>       \n\t\t\t<tr *ngFor=\"let item of itemsinp\" class=\"row-cart-item\">\n  <td class=\"vertical-align-center\">\n<img class=\"image-preview-card\" src={{item.url}}>\n<b class=\"bg-4\"> {{item.name}}</b> <button (click)=\"removeit(item)\" class=\"delete-button\"><span class=\"glyphicon glyphicon-trash\"></span></button>\n</td>\n<td class=\"vertical-align-center\"><p class=\"bg-4\">{{item.itemPrice}}</p></td>\n<td class=\"vertical-align-center\"><div class=\"cart-item-quantity\">\n<button (click)=\"decrement(item)\" class=\"quantity-change\" type=\"button\">-</button>\n<input [(ngModel)]=\"item.quantity\" class=\"input-quantity\" value=\"{{item.quantity}}\"/>\n<button (click)=\"increment(item)\" class=\"quantity-change\" type=\"button\">+</button></div></td>\n<td class=\"vertical-align-center\"><p class=\"bg-4\">{{item.quantity*item.itemPrice}}</p></td>\n</tr>\n</tbody>\n</table>\n\n  " }), 
        __metadata('design:paramtypes', [])
    ], TableComponent);
    return TableComponent;
}());
exports.TableComponent = TableComponent;
//# sourceMappingURL=table.component.js.map