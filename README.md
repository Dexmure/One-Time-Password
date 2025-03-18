# One-Time-Password
One Time Password

## Application du client:
Lorsque vous lancez l’application, vous pouvez directement voir votre jeton. Vous avez l’option de le copier dans le presse-papier avec le bouton « Copier ». Enfin, un compte à rebours vous indique le temps restant avant la génération du prochain jeton. Notez que le compte à rebours est de 30 secondes.
![Client](Images/client.png)

---

## Application côté serveur :
Lorsque vous entrez dans l’application serveur, vous avez la possibilité d’écrire votre jeton récupéré dans l’application client dans les champs sous « Veuillez entrer votre jeton : ». Si vous avez copié votre jeton dans votre presse-papier, vous pouvez le coller dans la première case dédiée à la vérification. Vous avez l’option d’effacer toutes les cases avec le bouton « Effacer ». Enfin, lorsque votre jeton est bien saisi, vous pouvez appuyer sur le bouton « Vérifier » avant que le compte à rebours de l’application client n’atteigne « 0 ». Si la vérification échoue, le nombre de tentatives diminue d’un. Notez que lorsque le nombre de tentatives atteint « 0 », l’application serveur se ferme et vous pouvez simplement la rouvrir.

![Client](Images/serveur.png)


## Je remercie mes collegues qui ont contribue a la reussite de cette application :
- Laurendeau Maxim
- Simard Samuel
