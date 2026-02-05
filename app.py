from __future__ import annotations

import os
from datetime import datetime
from typing import Optional

from flask import Flask, flash, redirect, render_template, request, session, url_for
from flask_sqlalchemy import SQLAlchemy
from werkzeug.security import check_password_hash, generate_password_hash

app = Flask(__name__)
app.config["SECRET_KEY"] = os.environ.get("SECRET_KEY", "dev-secret-key")
app.config["SQLALCHEMY_DATABASE_URI"] = "sqlite:///app.db"
app.config["SQLALCHEMY_TRACK_MODIFICATIONS"] = False

db = SQLAlchemy(app)


class User(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    username = db.Column(db.String(80), unique=True, nullable=False)
    password_hash = db.Column(db.String(255), nullable=False)
    points = db.Column(db.Integer, default=0, nullable=False)
    created_at = db.Column(db.DateTime, default=datetime.utcnow, nullable=False)


class Challenge(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    title = db.Column(db.String(120), nullable=False)
    description = db.Column(db.Text, nullable=False)
    points = db.Column(db.Integer, nullable=False)


class UserChallenge(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)
    challenge_id = db.Column(db.Integer, db.ForeignKey("challenge.id"), nullable=False)
    completed_at = db.Column(db.DateTime, default=datetime.utcnow, nullable=False)

    user = db.relationship("User", backref="completions")
    challenge = db.relationship("Challenge", backref="completions")


def current_user() -> Optional[User]:
    user_id = session.get("user_id")
    if user_id is None:
        return None
    return User.query.get(user_id)


@app.before_request
def ensure_database() -> None:
    db.create_all()
    if Challenge.query.count() == 0:
        seed_challenges()


@app.route("/")
def index():
    user = current_user()
    top_users = User.query.order_by(User.points.desc()).limit(5).all()
    return render_template("index.html", user=user, top_users=top_users)


@app.route("/register", methods=["GET", "POST"])
def register():
    if request.method == "POST":
        username = request.form.get("username", "").strip()
        password = request.form.get("password", "")
        if not username or not password:
            flash("Введите имя пользователя и пароль.", "error")
            return render_template("register.html")

        if User.query.filter_by(username=username).first():
            flash("Пользователь с таким именем уже существует.", "error")
            return render_template("register.html")

        user = User(username=username, password_hash=generate_password_hash(password))
        db.session.add(user)
        db.session.commit()
        session["user_id"] = user.id
        flash("Регистрация завершена. Добро пожаловать!", "success")
        return redirect(url_for("dashboard"))

    return render_template("register.html")


@app.route("/login", methods=["GET", "POST"])
def login():
    if request.method == "POST":
        username = request.form.get("username", "").strip()
        password = request.form.get("password", "")
        user = User.query.filter_by(username=username).first()

        if user is None or not check_password_hash(user.password_hash, password):
            flash("Неверные учетные данные.", "error")
            return render_template("login.html")

        session["user_id"] = user.id
        flash("Вы вошли в систему.", "success")
        return redirect(url_for("dashboard"))

    return render_template("login.html")


@app.route("/logout")
def logout():
    session.pop("user_id", None)
    flash("Вы вышли из системы.", "success")
    return redirect(url_for("index"))


@app.route("/dashboard")
def dashboard():
    user = current_user()
    if user is None:
        return redirect(url_for("login"))

    completed_ids = {completion.challenge_id for completion in user.completions}
    challenges = Challenge.query.order_by(Challenge.points.desc()).all()
    return render_template(
        "dashboard.html",
        user=user,
        challenges=challenges,
        completed_ids=completed_ids,
    )


@app.route("/challenges")
def challenges():
    user = current_user()
    if user is None:
        return redirect(url_for("login"))

    completed_ids = {completion.challenge_id for completion in user.completions}
    challenges_list = Challenge.query.order_by(Challenge.points.desc()).all()
    return render_template(
        "challenges.html",
        user=user,
        challenges=challenges_list,
        completed_ids=completed_ids,
    )


@app.route("/complete/<int:challenge_id>", methods=["POST"])
def complete_challenge(challenge_id: int):
    user = current_user()
    if user is None:
        return redirect(url_for("login"))

    challenge = Challenge.query.get_or_404(challenge_id)
    existing = UserChallenge.query.filter_by(user_id=user.id, challenge_id=challenge.id).first()
    if existing is not None:
        flash("Этот челлендж уже выполнен.", "info")
        return redirect(url_for("dashboard"))

    completion = UserChallenge(user_id=user.id, challenge_id=challenge.id)
    user.points += challenge.points
    db.session.add(completion)
    db.session.commit()
    flash(f"Челлендж выполнен! +{challenge.points} очков.", "success")
    return redirect(url_for("dashboard"))


@app.route("/leaderboard")
def leaderboard():
    user = current_user()
    leaders = User.query.order_by(User.points.desc(), User.created_at.asc()).all()
    return render_template("leaderboard.html", user=user, leaders=leaders)


def seed_challenges() -> None:
    starter_challenges = [
        Challenge(
            title="Утренняя зарядка",
            description="Сделайте 10 минут разминки и запишите результат.",
            points=15,
        ),
        Challenge(
            title="Прогулка 5 000 шагов",
            description="Пройдите минимум 5 000 шагов сегодня.",
            points=25,
        ),
        Challenge(
            title="Учебный спринт",
            description="Посвятите 30 минут изучению новой темы.",
            points=20,
        ),
        Challenge(
            title="Доброе дело",
            description="Сделайте маленькое доброе дело и опишите его другу.",
            points=30,
        ),
    ]
    db.session.add_all(starter_challenges)
    db.session.commit()


if __name__ == "__main__":
    app.run(debug=True)
